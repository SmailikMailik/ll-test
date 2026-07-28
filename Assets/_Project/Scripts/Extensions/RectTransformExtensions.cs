using UnityEngine;

namespace LL.Extensions
{
    internal static class RectTransformExtensions
    {
        internal static void SetRange(this RectTransform rectTransform, float from, float to)
        {
            rectTransform.anchorMin = new Vector2(from, 0f);
            rectTransform.anchorMax = new Vector2(to, 1f);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}