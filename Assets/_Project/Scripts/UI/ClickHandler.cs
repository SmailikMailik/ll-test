using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LL.UI
{
    internal sealed class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        internal Observable<Unit> Clicked => _clicked;

        private readonly Subject<Unit> _clicked = new();

        public void OnPointerClick(PointerEventData eventData) => _clicked.OnNext(Unit.Default);

        private void OnDestroy() => _clicked.Dispose();
    }
}