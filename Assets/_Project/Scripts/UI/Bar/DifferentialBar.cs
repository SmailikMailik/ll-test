using LL.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Bar
{
    internal sealed class DifferentialBar : MonoBehaviour
    {
        [SerializeField] private Slider _defaultSlider;
        [SerializeField] private Slider _currentSlider;

        internal void SetValue(float defaultValue, float currentValue, float maxValue)
        {
            SetSlider(_defaultSlider, defaultValue, maxValue);
            SetSlider(_currentSlider, currentValue, maxValue);
        }

        private static void SetSlider(Slider slider, float value, float maxValue)
        {
            slider.value = value.Normalize(maxValue);
        }
    }
}