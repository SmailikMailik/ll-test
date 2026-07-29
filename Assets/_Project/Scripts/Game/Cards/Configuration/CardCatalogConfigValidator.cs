using LL.Game.Data.Validation;
using LL.Game.Identifiers;

namespace LL.Game.Cards.Configuration
{
    internal sealed class CardCatalogConfigValidator : IDataValidator<CardDefinitionEntry[]>
    {
        private const string ExperienceAmountCode = "card.experience.positive";

        public void Validate(
            CardDefinitionEntry[] cards,
            ValidationContext context)
        {
            IdentifierCollectionValidator.Validate(
                cards,
                card => card.Id,
                context);

            if (cards == null)
                return;

            for (var index = 0; index < cards.Length; index++)
            {
                var card = cards[index];

                if (card == null)
                    continue;

                ValidationRules.Positive(
                    card.ExperienceAmount,
                    context.At(index).At("ExperienceAmount"),
                    ExperienceAmountCode);
            }
        }
    }
}