using UnityEngine;

namespace LL.UI
{
    [DisallowMultipleComponent]
    internal sealed class SafeArea : RectMonoBehaviour
    {
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;

        private void OnEnable()
        {
            Refresh(
                Screen.safeArea,
                new Vector2Int(Screen.width, Screen.height));
        }

        private void Update()
        {
            var safeArea = Screen.safeArea;
            var screenSize = new Vector2Int(Screen.width, Screen.height);

            if (safeArea != _lastSafeArea || screenSize != _lastScreenSize)
                Refresh(safeArea, screenSize);
        }

        private void Refresh(Rect safeArea, Vector2Int screenSize)
        {
            _lastSafeArea = safeArea;
            _lastScreenSize = screenSize;

            var horizontalInset = Mathf.Max(safeArea.xMin, screenSize.x - safeArea.xMax);
            safeArea.xMin = horizontalInset;
            safeArea.xMax = screenSize.x - horizontalInset;

            var verticalInset = Mathf.Max(safeArea.yMin, screenSize.y - safeArea.yMax);
            safeArea.yMin = verticalInset;
            safeArea.yMax = screenSize.y - verticalInset;

            var width = Mathf.Max(1, screenSize.x);
            var height = Mathf.Max(1, screenSize.y);

            RectTransform.anchorMin = new Vector2(safeArea.xMin / width, safeArea.yMin / height);
            RectTransform.anchorMax = new Vector2(safeArea.xMax / width, safeArea.yMax / height);
            RectTransform.offsetMin = Vector2.zero;
            RectTransform.offsetMax = Vector2.zero;
        }
    }
}