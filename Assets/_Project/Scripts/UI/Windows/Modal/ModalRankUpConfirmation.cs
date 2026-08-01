using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.RankUp;
using LL.Presentation.Localization;
using LL.Presentation.Payments;
using LL.Presentation.RankUp;
using LL.Presentation.Typography;
using VContainer;

namespace LL.UI.Windows.Modal
{
    internal sealed class ModalRankUpConfirmation : IRankUpConfirmation
    {
        private const string PriceVariable = "price";
        private const string HeroVariable = "hero";

        private readonly WindowController _windowController;
        private readonly ILocalizationService _localization;
        private readonly HeroCatalog _heroes;

        [Inject]
        internal ModalRankUpConfirmation(
            WindowController windowController,
            ILocalizationService localization,
            HeroCatalog heroes)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
        }

        public void Confirm(
            HeroId heroId,
            RankUpOptionDefinition option,
            Action onConfirmed,
            Action onRejected)
        {
            var hero = _heroes.GetHero(heroId);
            var payment = GetPayment(option);
            var priceText = payment is null ? string.Empty : PaymentFormatter.Format(payment.Value);
            var priceLabel = TextTags.Style(
                _localization.GetText(RankUpLocalizationKeys.PriceLabel),
                TextStyle.Muted);
            var priceLine = $"{priceLabel} {priceText}";
            var heroText = TextTags.Style(
                _localization.GetText(hero.NameLocalizationKey),
                TextStyle.Accent);

            _windowController.Show(new ModalWindowParameters
            (
                headerText: _localization.GetText(RankUpLocalizationKeys.Title),
                messageText: _localization.GetText(
                    RankUpLocalizationKeys.Confirmation,
                    new Dictionary<string, object>
                    {
                        [PriceVariable] = priceLine,
                        [HeroVariable] = heroText
                    }),
                positiveText: _localization.GetText(RankUpLocalizationKeys.ConfirmAction),
                positiveCallback: onConfirmed,
                negativeText: _localization.GetText(RankUpLocalizationKeys.CancelAction),
                negativeCallback: onRejected,
                closeCallback: onRejected
            ));
        }

        private static LL.Game.Payments.Payment? GetPayment(RankUpOptionDefinition option)
        {
            if (option is null)
                throw new ArgumentNullException(nameof(option));

            foreach (var requirement in option.Requirements)
            {
                if (requirement is PaymentRankUpRequirementDefinition payment)
                    return payment.Payment;
            }

            return null;
        }
    }
}