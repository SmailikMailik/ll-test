using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.Game.Ranks
{
    internal sealed class RankCatalog
    {
        internal IReadOnlyList<int> ExperienceRequirements { get; }

        internal RankCatalog(IEnumerable<int> experienceRequirements)
        {
            if (experienceRequirements == null)
                throw new ArgumentNullException(nameof(experienceRequirements));

            var requirements = experienceRequirements.ToArray();

            if (requirements.Length == 0)
                throw new ArgumentException(
                    "Rank catalog must contain at least one experience requirement.",
                    nameof(experienceRequirements));

            if (requirements[0] != 0)
                throw new ArgumentException(
                    "Rank 1 required experience must be zero.",
                    nameof(experienceRequirements));

            for (var index = 1; index < requirements.Length; index++)
            {
                if (requirements[index] <= 0)
                    throw new ArgumentException(
                        $"Rank {index + 1} required experience must be greater than zero.",
                        nameof(experienceRequirements));
            }

            ExperienceRequirements = Array.AsReadOnly(requirements);
        }
    }
}