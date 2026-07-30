using System;
using LL.UI.Controls;
using R3;
using TMPro;
using UnityEngine;

namespace LL.UI.Windows.Modal
{
    internal sealed class ModalWindow : Window<ModalWindowParameters>
    {
        [SerializeField] private TMP_Text _headerLabel;
        [SerializeField] private TMP_Text _messageLabel;
        [SerializeField] private TMP_Text _positiveLabel;
        [SerializeField] private TMP_Text _negativeLabel;

        [SerializeField] private InteractiveButton _positiveButton;
        [SerializeField] private InteractiveButton _negativeButton;
        [SerializeField] private InteractiveButton _closeButton;

        internal override bool CanClose => Parameters.CanClose;

        private void Start()
        {
            _positiveButton.Clicked.Subscribe(_ => OnButtonClicked(Parameters.PositiveCallback)).AddTo(this);
            _negativeButton.Clicked.Subscribe(_ => OnButtonClicked(Parameters.NegativeCallback)).AddTo(this);
            _closeButton.Clicked.Subscribe(_ => OnButtonClicked(Parameters.CloseCallback)).AddTo(this);
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

        private void OnButtonClicked(Action callback)
        {
            // The argument captures the current callback before TryClose can replace Parameters.
            TryClose();
            callback?.Invoke();
        }
    }
}