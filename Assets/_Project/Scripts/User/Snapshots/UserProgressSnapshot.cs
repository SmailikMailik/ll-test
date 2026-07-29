using System;

namespace LL.User.Snapshots
{
    internal sealed class UserProgressSnapshot
    {
        internal int Rank { get; }
        internal int Experience { get; }

        internal UserProgressSnapshot(int rank, int experience)
        {
            Rank = Math.Max(1, rank);
            Experience = Math.Max(0, experience);
        }
    }
}