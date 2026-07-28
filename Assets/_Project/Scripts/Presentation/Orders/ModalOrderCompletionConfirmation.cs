using System;
using LL.Presentation.Localization;
using LL.UI.Windows;
using LL.UI.Windows.Views;
using VContainer;

namespace LL.Presentation.Orders
{
    internal sealed class ModalOrderCompletionConfirmation : IOrderCompletionConfirmation
    {
        private readonly WindowController _windowController;
        private readonly ILocalizationService _localization;

        [Inject]
        internal ModalOrderCompletionConfirmation(
            WindowController windowController,
            ILocalizationService localization)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public void Confirm(Action onConfirmed)
        {
            _windowController.Show(new ModalWindowParameters
            (
                headerText: _localization.GetText(OrderLocalizationKeys.TestingTitle),
                messageText: _localization.GetText(OrderLocalizationKeys.TestingMessage),
                positiveText: _localization.GetText(OrderLocalizationKeys.CompleteAction),
                positiveCallback: onConfirmed,
                negativeText: _localization.GetText(OrderLocalizationKeys.CancelAction),
                closeActive: false
            ));
        }
    }
}