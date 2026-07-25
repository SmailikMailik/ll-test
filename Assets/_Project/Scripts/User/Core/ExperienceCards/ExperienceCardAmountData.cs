using System;
using LL.Game.ExperienceCards;

namespace LL.User.Core.ExperienceCards
{
    internal sealed class ExperienceCardAmountData
    {
        internal ExperienceCardId Id { get; }
        internal int Amount { get; }

        internal ExperienceCardAmountData(ExperienceCardId id, int amount)
        {
            Id = id;
            Amount = Math.Max(0, amount);
        }
    }
}