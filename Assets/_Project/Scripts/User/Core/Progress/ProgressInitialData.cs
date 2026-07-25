using System;

namespace LL.User.Core.Progress
{
    internal sealed class ProgressInitialData
    {
        internal int TotalExperience { get; }

        internal ProgressInitialData(int totalExperience)
        {
            TotalExperience = Math.Max(0, totalExperience);
        }
    }
}