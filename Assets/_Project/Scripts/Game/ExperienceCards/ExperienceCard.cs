using System;

namespace LL.Game.ExperienceCards
{
    internal sealed class ExperienceCard : IExperienceCard
    {
        public ExperienceCardId Id { get; }
        public int ExperienceAmount { get; }
        public int MaxAmount { get; }

        internal ExperienceCard(
            ExperienceCardId id,
            int experienceAmount,
            int maxAmount)
        {
            Id = id;
            ExperienceAmount = Math.Max(1, experienceAmount);
            MaxAmount = Math.Max(0, maxAmount);
        }
    }
}