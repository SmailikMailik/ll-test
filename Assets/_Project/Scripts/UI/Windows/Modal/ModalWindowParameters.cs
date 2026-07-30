using System;

namespace LL.UI.Windows.Modal
{
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