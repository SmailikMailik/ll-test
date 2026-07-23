using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Bar.Segmented
{
    internal sealed class SegmentedBarItem : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _foregroundImage;

        internal void Enable()
        {
            _backgroundImage.gameObject.SetActive(false);
            _foregroundImage.gameObject.SetActive(true);
        }

        internal void Disable()
        {
            _backgroundImage.gameObject.SetActive(true);
            _foregroundImage.gameObject.SetActive(false);
        }
    }
}