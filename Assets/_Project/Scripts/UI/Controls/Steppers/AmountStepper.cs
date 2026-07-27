using System;
using LL.UI.Controls.Buttons;
using LL.UI.Formatting;
using R3;
using TMPro;
using UnityEngine;

namespace LL.UI.Controls.Steppers
{
    [DisallowMultipleComponent]
    internal sealed class AmountStepper : MonoBehaviour
    {
        [SerializeField] private InteractiveButton _decreaseButton;
        [SerializeField] private TMP_Text _decreaseButtonLabel;

        [SerializeField] private InteractiveButton _increaseButton;
        [SerializeField] private TMP_Text _increaseButtonLabel;

        [SerializeField] private TMP_Text _valueLabel;

        private const int Minimum = 0;

        internal Observable<int> ValueChanged => _valueChanged;

        private readonly Subject<int> _valueChanged = new();

        private int _value;
        private int _maximum;
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

        internal void SetValue(int value, int maximum)
        {
            var nextMaximum = Math.Max(Minimum, maximum);
            var nextValue = Math.Clamp(value, Minimum, nextMaximum);

            if (_value == nextValue && _maximum == nextMaximum)
                return;

            var valueChanged = _value != nextValue;

            _value = nextValue;
            _maximum = nextMaximum;

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

            button.Clicked
                .Subscribe(_ => ChangeValue(delta))
                .AddTo(this);
        }

        private void ChangeValue(int delta)
        {
            if (CanChangeValue(delta))
                SetValue(_value + delta, _maximum);
        }

        private void Refresh()
        {
            _valueLabel.text = TextFormatter.Number(_value);
            _decreaseButton.SetInteractable(CanChangeValue(-_delta));
            _increaseButton.SetInteractable(CanChangeValue(_delta));
        }

        private bool CanChangeValue(int delta) => delta switch
        {
            > 0 => _value <= _maximum - delta,
            < 0 => _value >= Minimum - delta,
            _ => false
        };
    }
}