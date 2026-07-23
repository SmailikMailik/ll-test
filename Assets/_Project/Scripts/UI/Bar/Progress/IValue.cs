using LL.Extensions;
using TMPro;
using UnityEngine;

namespace LL.UI.Bar.Progress
{
    internal enum ValueType : byte
    {
        None = 0,
        Integer = 1,
        Floating = 2,
        Percent = 3
    }

    internal interface IValue
    {
        void SetText(float value, float maxValue);
    }

    internal sealed class ValueMock : IValue
    {
        public void SetText(float value, float maxValue) { }
    }

    internal sealed class ValueInteger : IValue
    {
        private readonly TMP_Text _valueLabel;
        private readonly string _valueFormat;

        internal ValueInteger(TMP_Text valueLabel, string valueFormat)
        {
            _valueLabel = valueLabel;
            _valueFormat = valueFormat;
        }

        public void SetText(float value, float maxValue)
        {
            _valueLabel.text = string.Format(_valueFormat, Mathf.Ceil(value), Mathf.Ceil(maxValue));
        }
    }

    internal sealed class ValueFloating : IValue
    {
        private readonly TMP_Text _valueLabel;
        private readonly string _valueFormat;

        internal ValueFloating(TMP_Text valueLabel, string valueFormat)
        {
            _valueLabel = valueLabel;
            _valueFormat = valueFormat;
        }

        public void SetText(float value, float maxValue)
        {
            _valueLabel.text = string.Format(_valueFormat, $"{value:F2}", $"{maxValue:F2}");
        }
    }

    internal sealed class ValuePercent : IValue
    {
        private readonly TMP_Text _valueLabel;
        private readonly string _valueFormat;

        internal ValuePercent(TMP_Text valueLabel, string valueFormat)
        {
            _valueLabel = valueLabel;
            _valueFormat = valueFormat;
        }

        public void SetText(float value, float maxValue)
        {
            _valueLabel.text = string.Format(_valueFormat, $"{value.ToPercent(maxValue)}");
        }
    }
}