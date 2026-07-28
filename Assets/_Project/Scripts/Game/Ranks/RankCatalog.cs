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
            var requirements = experienceRequirements?.ToArray() ?? Array.Empty<int>();

            if (requirements.Length == 0)
                requirements = new[] { 0 };

            requirements[0] = 0;

            for (var index = 1; index < requirements.Length; index++)
                requirements[index] = Math.Max(1, requirements[index]);

            ExperienceRequirements = Array.AsReadOnly(requirements);
        }
    }
}