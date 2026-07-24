using System;
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

        protected override bool CanGoBack => Parameters.CanClose;

        private void OnEnable()
        {
            _positiveButton.Clicked += OnPositiveClicked;
            _negativeButton.Clicked += OnNegativeClicked;
            _closeButton.Clicked += OnCloseClicked;
        }

        private void OnDisable()
        {
            _positiveButton.Clicked -= OnPositiveClicked;
            _negativeButton.Clicked -= OnNegativeClicked;
            _closeButton.Clicked -= OnCloseClicked;
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

        private void OnPositiveClicked()
        {
            Back();
            Parameters.PositiveCallback?.Invoke();
        }

        private void OnNegativeClicked()
        {
            Back();
            Parameters.NegativeCallback?.Invoke();
        }

        private void OnCloseClicked()
        {
            Back();
            Parameters.CloseCallback?.Invoke();
        }
    }

    internal sealed class ModalWindowParameters : IWindowParameters
    {
        internal string HeaderText { get; set; } = string.Empty;
        internal string MessageText { get; set; } = string.Empty;

        internal string PositiveText { get; set; } = string.Empty;
        internal Action PositiveCallback { get; set; } = null;
        internal bool PositiveActive { get; set; } = true;

        internal string NegativeText { get; set; } = string.Empty;
        internal Action NegativeCallback { get; set; } = null;
        internal bool NegativeActive { get; set; } = true;

        internal Action CloseCallback { get; set; } = null;
        internal bool CloseActive { get; set; } = true;

        internal bool CanClose { get; set; } = true;
    }
}