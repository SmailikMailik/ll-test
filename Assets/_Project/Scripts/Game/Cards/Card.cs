using System;
using LL.Game.Items;

namespace LL.Game.Cards
{
    internal sealed class Card : ICard
    {
        public ItemId Id { get; }
        public int ExperienceAmount { get; }

        internal Card(ItemId id, int experienceAmount)
        {
            if (id.IsEmpty)
                throw new ArgumentException("Card ID must be non-empty.", nameof(id));

            if (experienceAmount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(experienceAmount),
                    experienceAmount,
                    "Card experience amount must be greater than zero.");

            Id = id;
            ExperienceAmount = experienceAmount;
        }
    }
}