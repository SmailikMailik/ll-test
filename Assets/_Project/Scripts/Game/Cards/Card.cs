using System;
using LL.Game.Identifiers;
using LL.Game.Items;
using LL.Validation;

namespace LL.Game.Cards
{
    internal sealed class Card : ICard
    {
        public ItemId Id { get; }
        public int ExperienceAmount { get; }

        internal Card(ItemId id, int experienceAmount)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));

            if (ValidationChecks.IsNonPositive(experienceAmount))
                throw new ArgumentOutOfRangeException(
                    nameof(experienceAmount),
                    experienceAmount,
                    "Card experience amount must be greater than zero.");

            Id = id;
            ExperienceAmount = experienceAmount;
        }
    }
}