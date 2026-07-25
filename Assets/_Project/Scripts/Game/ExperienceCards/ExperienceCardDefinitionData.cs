using System;

namespace LL.Game.ExperienceCards
{
    internal sealed class ExperienceCardDefinitionData
    {
        internal ExperienceCardId Id { get; }
        internal int ExperienceAmount { get; }
        internal int Capacity { get; }

        internal ExperienceCardDefinitionData(
            ExperienceCardId id,
            int experienceAmount,
            int capacity)
        {
            Id = id;
            ExperienceAmount = Math.Max(1, experienceAmount);
            Capacity = Math.Max(0, capacity);
        }
    }
}