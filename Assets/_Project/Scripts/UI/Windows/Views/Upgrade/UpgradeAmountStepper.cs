using System;
using LL.UI.Controls.Buttons;
using LL.UI.Formatting;
using R3;
using TMPro;
using UnityEngine;

namespace LL.UI.Windows.Views.Upgrade
{
    [DisallowMultipleComponent]
    internal sealed class UpgradeAmountStepper : MonoBehaviour
    {
        [SerializeField] private InteractiveButton _decreaseButton;
        [SerializeField] private TMP_Text _decreaseButtonLabel;

        [SerializeField] private InteractiveButton _increaseButton;
        [SerializeField] private TMP_Text _increaseButtonLabel;

        [SerializeField] private TMP_Text _valueLabel;

        private const int MinimumAmount = 0;

        internal int Value { get; private set; }
        internal Observable<int> ValueChanged => _valueChanged;

        private readonly Subject<int> _valueChanged = new();
        private int _maximum;
        private int _delta;
        private bool _isInitialized;

        internal void Initialize(int delta)
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(UpgradeAmountStepper)} is already initialized.");

            if (delta <= 0)
                throw new ArgumentOutOfRangeException(nameof(delta));

            _isInitialized = true;
            _delta = delta;

            InitializeButton(_decreaseButton, _decreaseButtonLabel, -_delta);
            InitializeButton(_increaseButton, _increaseButtonLabel, _delta);
            Refresh();
        }

        private void OnDestroy()
        {
            _valueChanged.Dispose();
        }

        internal void ResetValue(int maximum = MinimumAmount)
        {
            _maximum = Math.Max(MinimumAmount, maximum);
            SetValue(MinimumAmount);
        }

        internal void SetMaximum(int maximum)
        {
            _maximum = Math.Max(MinimumAmount, maximum);
            SetValue(Math.Clamp(Value, MinimumAmount, _maximum));
        }

        private void InitializeButton(
            InteractiveButton button,
            TMP_Text label,
            int delta)
        {
            if (delta == 0)
                throw new ArgumentOutOfRangeException(nameof(delta));

            var sign = delta > 0 ? "+" : "-";
            label.text = $"{sign}{Math.Abs(delta)}";

            button.Clicked
                .Subscribe(_ => ChangeValue(delta))
                .AddTo(this);
        }

        private void ChangeValue(int delta)
        {
            if (CanChangeValue(delta))
                SetValue(Value + delta);
        }

        private void SetValue(int value)
        {
            var changed = Value != value;
            Value = value;
            Refresh();

            if (changed)
                _valueChanged.OnNext(Value);
        }

        private void Refresh()
        {
            _valueLabel.text = TextFormatter.Number(Value);
            _decreaseButton.SetInteractable(CanChangeValue(-_delta));
            _increaseButton.SetInteractable(CanChangeValue(_delta));
        }

        private bool CanChangeValue(int delta) => delta switch
        {
            > 0 => Value <= _maximum - delta,
            < 0 => Value >= MinimumAmount - delta,
            _ => false
        };
    }
}