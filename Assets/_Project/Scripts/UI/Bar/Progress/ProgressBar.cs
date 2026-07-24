using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Bar.Progress
{
    internal sealed class ProgressBar : MonoBehaviour
    {
        [SerializeField] private StripType _stripType = StripType.Common;
        [SerializeField] private Image _foregroundImage;
        [SerializeField] private Image _shadowImage;

        [SerializeField] private ValueType _valueType = ValueType.None;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private string _valueFormat = "{0} / {1}";

        private IStrip _strip;
        private IValue _value;

        internal void Awake()
        {
            var strips = new Dictionary<StripType, IStrip>
            {
                [StripType.Common] = new StripCommon(_foregroundImage),
                [StripType.Shadow] = new StripShadow(this, _foregroundImage, _shadowImage)
            };

            var values = new Dictionary<ValueType, IValue>
            {
                [ValueType.None] = new ValueMock(),
                [ValueType.Integer] = new ValueInteger(_valueLabel, _valueFormat),
                [ValueType.Floating] = new ValueFloating(_valueLabel, _valueFormat),
                [ValueType.Percent] = new ValuePercent(_valueLabel, _valueFormat)
            };

            _strip = strips[_stripType];
            _value = values[_valueType];
        }

        internal void SetValue(float value, float minValue, float maxValue)
        {
            value = Mathf.Clamp(value, minValue, maxValue);

            _strip.SetFillAmount(value, maxValue);
            _value.SetText(value, maxValue);
        }
    }
}