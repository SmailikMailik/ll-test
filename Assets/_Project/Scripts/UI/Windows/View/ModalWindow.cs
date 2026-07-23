using System;
using LL.UI.Windows.Core;
using TMPro;
using UnityEngine;

namespace LL.UI.Windows.View
{
    internal sealed class ModalWindow : WindowParameterized<ModalParameters>
    {
        [SerializeField] private TMP_Text _headerLabel;
        [SerializeField] private TMP_Text _messageLabel;
        [SerializeField] private TMP_Text _positiveLabel;
        [SerializeField] private TMP_Text _negativeLabel;

        [SerializeField] private CommonButton _positiveButton;
        [SerializeField] private CommonButton _negativeButton;
        [SerializeField] private CommonButton _closeButton;

        internal override void Init()
        {
            base.Init();

            _positiveButton.Clicked += () =>
            {
                Back();
                Parameters.PositiveCallback?.Invoke();
            };

            _negativeButton.Clicked += () =>
            {
                Back();
                Parameters.NegativeCallback?.Invoke();
            };

            _closeButton.Clicked += () =>
            {
                Back();
                Parameters.CloseCallback?.Invoke();
            };
        }

        internal override void Show()
        {
            base.Show();

            _positiveButton.gameObject.SetActive(Parameters.PositiveActive);
            _negativeButton.gameObject.SetActive(Parameters.NegativeActive);
            _closeButton.gameObject.SetActive(Parameters.CloseActive);

            _headerLabel.text = Parameters.HeaderText;
            _messageLabel.text = Parameters.MessageText;
            _positiveLabel.text = Parameters.PositiveText;
            _negativeLabel.text = Parameters.NegativeText;
        }

        internal override void Back()
        {
            if (Parameters.CanClose)
                base.Back();
        }
    }

    internal sealed class ModalParameters : IWindowParameters
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