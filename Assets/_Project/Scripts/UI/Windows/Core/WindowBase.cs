using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Core
{
    [RequireComponent(typeof(Canvas))]
    internal class WindowBase : RectMonoBehaviour
    {
        [Inject] internal WindowsController WindowsController { get; private set; }

        internal bool IsActive { get; private set; }

        private Canvas _canvas;

        internal virtual void Init()
        {
            _canvas = GetComponent<Canvas>();
        }

        internal virtual void Show()
        {
            IsActive = true;
            _canvas.enabled = true;
        }

        internal virtual void Hide()
        {
            IsActive = false;
            _canvas.enabled = false;
        }

        internal virtual void Back()
        {
            WindowsController.Back();
        }
    }
}