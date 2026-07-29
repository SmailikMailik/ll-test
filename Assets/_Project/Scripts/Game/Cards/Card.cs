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
            Id = id;
            ExperienceAmount = Math.Max(1, experienceAmount);
        }
    }
}