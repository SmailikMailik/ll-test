using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.User.Core.Progress
{
    internal sealed class RankProgressionData
    {
        internal IReadOnlyList<int> ExperienceThresholds { get; }

        internal RankProgressionData(IEnumerable<int> experienceThresholds)
        {
            var copy = experienceThresholds?.ToArray() ?? Array.Empty<int>();
            ExperienceThresholds = Array.AsReadOnly(copy);
        }
    }
}