using LL.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI
{
    internal sealed class CommonSlider : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        internal void Init(ProxyParameter<float> proxyParameter, float min, float max)
        {
            _slider.SetupSlider(proxyParameter.GetValue(), min, max);
            _slider.onValueChanged.AddListener(proxyParameter.SetValue);
        }
    }
}