using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.User.Core.Progress
{
    internal sealed class LevelProgressionData
    {
        internal IReadOnlyList<int> ExperienceThresholds { get; }

        internal LevelProgressionData(IEnumerable<int> experienceThresholds)
        {
            var copy = experienceThresholds?.ToArray() ?? Array.Empty<int>();
            ExperienceThresholds = Array.AsReadOnly(copy);
        }
    }
}