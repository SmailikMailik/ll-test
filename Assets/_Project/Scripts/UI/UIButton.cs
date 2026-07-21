using System;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI
{
    internal sealed class UIButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        internal event Action Clicked;

        internal bool Interactable => _button.interactable;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            if (Interactable is false)
                return;

            Clicked?.Invoke();
        }
    }
}