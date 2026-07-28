using System;
using System.Collections.Generic;
using LL.Presentation.Localization;
using LL.Purchasing;
using LL.UI.Typography;
using LL.UI.Windows;
using LL.UI.Windows.Views;
using VContainer;

namespace LL.Presentation.Promotions
{
    internal sealed class ModalRankPromotionConfirmation : IPurchaseConfirmation
    {
        private const string PriceVariable = "price";
        private const string HeroVariable = "hero";

        private readonly WindowController _windowController;
        private readonly ILocalizationService _localization;

        [Inject]
        internal ModalRankPromotionConfirmation(
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
            var priceLabel = TextTags.Style(
                _localization.GetText(RankPromotionLocalizationKeys.PriceLabel),
                TextStyle.Muted);
            var priceLine = $"{priceLabel} {priceText}";
            var heroText = TextTags.Style(
                _localization.GetText(RankPromotionLocalizationKeys.Hero),
                TextStyle.Accent);

            _windowController.Show(new ModalWindowParameters
            (
                headerText: _localization.GetText(RankPromotionLocalizationKeys.Title),
                messageText: _localization.GetText(
                    RankPromotionLocalizationKeys.Confirmation,
                    new Dictionary<string, object>
                    {
                        [PriceVariable] = priceLine,
                        [HeroVariable] = heroText
                    }),
                positiveText: _localization.GetText(RankPromotionLocalizationKeys.ConfirmAction),
                positiveCallback: onConfirmed,
                negativeText: _localization.GetText(RankPromotionLocalizationKeys.CancelAction),
                negativeCallback: onRejected,
                closeCallback: onRejected
            ));
        }
    }
}