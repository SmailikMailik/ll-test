using System;

namespace LL.User.Core.Progress
{
    internal sealed class UserProgressInitialData
    {
        internal int Rank { get; }
        internal int Experience { get; }

        internal UserProgressInitialData(int rank, int experience)
        {
            Rank = Math.Max(1, rank);
            Experience = Math.Max(0, experience);
        }
    }
}