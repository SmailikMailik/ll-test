using System;
using System.Collections.Generic;
using LL.Game.Purchases;
using LL.Presentation.Localization;
using LL.Purchasing;
using LL.UI.Typography;
using LL.UI.Windows;
using LL.UI.Windows.Views;
using VContainer;

namespace LL.Presentation.Purchasing
{
    internal sealed class ModalPurchaseConfirmation : IPurchaseConfirmation
    {
        private const string PriceVariable = "price";
        private const string HeroVariable = "hero";

        private readonly WindowController _windowController;
        private readonly ILocalizationService _localization;

        [Inject]
        internal ModalPurchaseConfirmation(
            WindowController windowController,
            ILocalizationService localization)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public void Confirm(IPurchase purchase, Action onConfirmed, Action onRejected)
        {
            if (purchase == null)
                throw new ArgumentNullException(nameof(purchase));

            var priceText = TextFormatter.ItemAmount(purchase.ItemId, purchase.Price);
            var heroText = _localization.GetText(PurchaseLocalizationKeys.Hero);

            _windowController.Show(new ModalWindowParameters
            (
                headerText: _localization.GetText(PurchaseLocalizationKeys.Title),
                messageText: _localization.GetText(
                    PurchaseLocalizationKeys.Confirmation,
                    new Dictionary<string, object>
                    {
                        [PriceVariable] = priceText,
                        [HeroVariable] = heroText
                    }),
                positiveText: _localization.GetText(PurchaseLocalizationKeys.ConfirmAction),
                positiveCallback: onConfirmed,
                negativeText: _localization.GetText(PurchaseLocalizationKeys.CancelAction),
                negativeCallback: onRejected,
                closeCallback: onRejected
            ));
        }
    }
}