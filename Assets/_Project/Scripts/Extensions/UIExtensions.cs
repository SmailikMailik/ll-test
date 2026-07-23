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
    }
}