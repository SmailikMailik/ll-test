using System;

namespace LL.Game.ExperienceCards
{
    internal sealed class ExperienceCard : IExperienceCard
    {
        public ExperienceCardId Id { get; }
        public int ExperienceAmount { get; }

        internal ExperienceCard(ExperienceCardId id, int experienceAmount)
        {
            Id = id;
            ExperienceAmount = Math.Max(1, experienceAmount);
        }
    }
}