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
            var copy = experienceRequirements?.ToArray() ?? Array.Empty<int>();
            ExperienceRequirements = Array.AsReadOnly(copy);
        }
    }
}