using System.Globalization;
using UnityEngine.UI;

namespace LL.Extensions
{
    internal static class UIExtensions
    {
        private static readonly NumberFormatInfo _numberFormat = new()
        {
            NumberGroupSeparator = "\u202F", // узкий неразрывный пробел
            NumberDecimalDigits = 0
        };

        internal static string ToNumber(this int value)
        {
            return value.ToString("N0", _numberFormat);
        }

        internal static void SetupSlider(this Slider slider, float value, float min, float max)
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;
        }
    }
}