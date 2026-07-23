using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Bar.Marked
{
    internal sealed class MarkedBarItem : MonoBehaviour
    {
        [SerializeField] private Image _markImage;

        internal void Enable()
        {
            _markImage.gameObject.SetActive(true);
        }

        internal void Disable()
        {
            _markImage.gameObject.SetActive(false);
        }
    }
}