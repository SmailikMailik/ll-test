using System;

namespace LL.Game.Ranks
{
    internal sealed class RankDefinition
    {
        internal RankId Id { get; }
        internal int Number { get; }
        internal int RequiredExperience { get; }

        internal RankDefinition(
            RankId id,
            int number,
            int requiredExperience)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
                throw new ArgumentException("Rank ID must be non-empty.", nameof(id));

            if (number < 1)
                throw new ArgumentOutOfRangeException(nameof(number));

            if (number == 1 && requiredExperience != 0)
                throw new ArgumentException("Rank 1 required experience must be zero.", nameof(requiredExperience));

            if (number > 1 && requiredExperience <= 0)
                throw new ArgumentOutOfRangeException(nameof(requiredExperience));

            Id = id;
            Number = number;
            RequiredExperience = requiredExperience;
        }
    }
}