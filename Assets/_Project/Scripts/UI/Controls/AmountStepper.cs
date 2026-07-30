using System;
using LL.Presentation.Typography;
using R3;
using TMPro;
using UnityEngine;

namespace LL.UI.Controls
{
    [DisallowMultipleComponent]
    internal sealed class AmountStepper : MonoBehaviour
    {
        [SerializeField] private InteractiveButton _decreaseButton;
        [SerializeField] private TMP_Text _decreaseButtonLabel;

        [SerializeField] private InteractiveButton _increaseButton;
        [SerializeField] private TMP_Text _increaseButtonLabel;

        [SerializeField] private TMP_Text _valueLabel;

        private const int Min = 0;

        internal Observable<int> ValueChanged => _valueChanged;

        private readonly Subject<int> _valueChanged = new();

        private int _value;
        private int _max;
        private int _delta;

        private bool _isInitialized;

        internal void Initialize(int delta)
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(AmountStepper)} is already initialized.");

            if (delta <= 0)
                throw new ArgumentOutOfRangeException(nameof(delta));

            _isInitialized = true;

            _delta = delta;
            _valueChanged.AddTo(this);

            InitializeButton(_decreaseButton, _decreaseButtonLabel, -_delta);
            InitializeButton(_increaseButton, _increaseButtonLabel, _delta);

            Refresh();
        }

        internal void SetValue(int value, int max)
        {
            var nextMax = Math.Max(Min, max);
            var nextValue = Math.Clamp(value, Min, nextMax);

            if (_value == nextValue && _max == nextMax)
                return;

            var valueChanged = _value != nextValue;

            _value = nextValue;
            _max = nextMax;

            Refresh();

            if (valueChanged)
                _valueChanged.OnNext(_value);
        }

        private void InitializeButton(InteractiveButton button, TMP_Text label, int delta)
        {
            if (delta == 0)
                throw new ArgumentOutOfRangeException(nameof(delta));

            var sign = delta > 0 ? "+" : "-";
            label.text = $"{sign}{Math.Abs(delta)}";

            button.Clicked.Subscribe(_ => ChangeValue(delta)).AddTo(this);
        }

        private void ChangeValue(int delta)
        {
            if (CanChangeValue(delta))
                SetValue(_value + delta, _max);
        }

        private void Refresh()
        {
            _valueLabel.text = TextFormatter.Number(_value);
            _decreaseButton.SetInteractable(CanChangeValue(-_delta));
            _increaseButton.SetInteractable(CanChangeValue(_delta));
        }

        private bool CanChangeValue(int delta) => delta switch
        {
            > 0 => _value <= _max - delta,
            < 0 => _value >= Min - delta,
            _ => false
        };
    }
}