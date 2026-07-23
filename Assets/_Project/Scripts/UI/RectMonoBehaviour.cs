using UnityEngine;

namespace LL.UI
{
    internal class RectMonoBehaviour : MonoBehaviour
    {
        internal RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == false)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        private RectTransform _rectTransform;
    }
}