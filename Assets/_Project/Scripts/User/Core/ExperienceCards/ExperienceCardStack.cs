using System;
using LL.Game.ExperienceCards;

namespace LL.User.Core.ExperienceCards
{
    internal sealed class ExperienceCardStack
    {
        internal ExperienceCardId Id { get; }
        internal int Amount { get; }

        internal ExperienceCardStack(ExperienceCardId id, int amount)
        {
            Id = id;
            Amount = Math.Max(0, amount);
        }
    }
}