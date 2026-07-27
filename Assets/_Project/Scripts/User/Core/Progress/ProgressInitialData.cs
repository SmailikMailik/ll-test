using System;

namespace LL.User.Core.Progress
{
    internal sealed class ProgressInitialData
    {
        internal int Rank { get; }
        internal int Experience { get; }

        internal ProgressInitialData(int rank, int experience)
        {
            Rank = Math.Max(1, rank);
            Experience = Math.Max(0, experience);
        }
    }
}