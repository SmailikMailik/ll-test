using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LL.UI
{
    internal sealed class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        internal event Action Clicked;

        public void OnPointerClick(PointerEventData eventData = null) => Clicked?.Invoke();
    }
}