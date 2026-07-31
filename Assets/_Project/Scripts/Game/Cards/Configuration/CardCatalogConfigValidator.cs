using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Cards.Configuration
{
    internal sealed class CardCatalogConfigValidator : IDataValidator<CardEntry[]>
    {
        private const string EntriesCode = "card.entries.required";
        private const string ExperienceAmountCode = "card.experience.positive";

        public void Validate(
            CardEntry[] cards,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(cards, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                cards,
                card => card.Id,
                context);

            for (var index = 0; index < cards.Length; index++)
            {
                var card = cards[index];

                if (card is null)
                    continue;

                var cardContext = context.At(index);
                ValidationRules.Positive(
                    card.ExperienceAmount,
                    cardContext.At(nameof(CardEntry.ExperienceAmount)),
                    ExperienceAmountCode);
            }
        }
    }
}