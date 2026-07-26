using System;

namespace LL.Game.Cards
{
    internal sealed class Card : ICard
    {
        public CardId Id { get; }
        public int ExperienceAmount { get; }

        internal Card(CardId id, int experienceAmount)
        {
            Id = id;
            ExperienceAmount = Math.Max(1, experienceAmount);
        }
    }
}