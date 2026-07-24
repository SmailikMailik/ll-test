using System;
using R3;
using TMPro;
using UnityEngine;

namespace LL.UI.Windows.Views
{
    internal sealed class ModalWindow : Window<ModalWindowParameters>
    {
        [SerializeField] private TMP_Text _headerLabel;
        [SerializeField] private TMP_Text _messageLabel;
        [SerializeField] private TMP_Text _positiveLabel;
        [SerializeField] private TMP_Text _negativeLabel;

        [SerializeField] private CommonButton _positiveButton;
        [SerializeField] private CommonButton _negativeButton;
        [SerializeField] private CommonButton _closeButton;

        internal override bool CanClose => Parameters.CanClose;

        private void Start()
        {
            _positiveButton.Clicked.Subscribe(_ => HandleClick(Parameters.PositiveCallback)).AddTo(this);
            _negativeButton.Clicked.Subscribe(_ => HandleClick(Parameters.NegativeCallback)).AddTo(this);
            _closeButton.Clicked.Subscribe(_ => HandleClick(Parameters.CloseCallback)).AddTo(this);
        }

        protected override void OnShow()
        {
            _positiveButton.gameObject.SetActive(Parameters.PositiveActive);
            _negativeButton.gameObject.SetActive(Parameters.NegativeActive);
            _closeButton.gameObject.SetActive(Parameters.CloseActive);

            _headerLabel.text = Parameters.HeaderText;
            _messageLabel.text = Parameters.MessageText;

            _positiveLabel.text = Parameters.PositiveText;
            _negativeLabel.text = Parameters.NegativeText;
        }

        private void HandleClick(Action callback)
        {
            // The argument captures the current callback before TryClose can replace Parameters.
            TryClose();
            callback?.Invoke();
        }
    }

    internal sealed class ModalWindowParameters : IWindowParameters
    {
        internal string HeaderText { get; }
        internal string MessageText { get; }

        internal string PositiveText { get; }
        internal Action PositiveCallback { get; }
        internal bool PositiveActive { get; }

        internal string NegativeText { get; }
        internal Action NegativeCallback { get; }
        internal bool NegativeActive { get; }

        internal Action CloseCallback { get; }
        internal bool CloseActive { get; }

        internal bool CanClose { get; }

        internal ModalWindowParameters(
            string headerText,
            string messageText,
            string positiveText = "",
            Action positiveCallback = null,
            bool positiveActive = true,
            string negativeText = "",
            Action negativeCallback = null,
            bool negativeActive = true,
            Action closeCallback = null,
            bool closeActive = true,
            bool canClose = true)
        {
            HeaderText = headerText ?? string.Empty;
            MessageText = messageText ?? string.Empty;

            PositiveText = positiveText ?? string.Empty;
            PositiveCallback = positiveCallback;
            PositiveActive = positiveActive;

            NegativeText = negativeText ?? string.Empty;
            NegativeCallback = negativeCallback;
            NegativeActive = negativeActive;

            CloseCallback = closeCallback;
            CloseActive = closeActive;

            CanClose = canClose;
        }
    }
}