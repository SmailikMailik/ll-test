using UnityEngine;
using UnityEngine.UI;

namespace LL.Extensions
{
    internal static class UIExtensions
    {
        internal static void SetupSlider(this Slider slider, float value, float min, float max)
        {
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = value;
        }

        internal static void SetRange(this RectTransform rectTransform, float from, float to)
        {
            rectTransform.anchorMin = new Vector2(from, 0f);
            rectTransform.anchorMax = new Vector2(to, 1f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}