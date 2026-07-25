using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.Game.Ranks
{
    internal sealed class RankCatalog
    {
        internal IReadOnlyList<int> ExperienceThresholds { get; }

        internal RankCatalog(IEnumerable<int> experienceThresholds)
        {
            var copy = experienceThresholds?.ToArray() ?? Array.Empty<int>();
            ExperienceThresholds = Array.AsReadOnly(copy);
        }
    }
}