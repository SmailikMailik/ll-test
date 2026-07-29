using System;

namespace LL.User.Snapshots
{
    internal sealed class UserProgressSnapshot
    {
        internal int Rank { get; }
        internal int Experience { get; }

        internal UserProgressSnapshot(int rank, int experience)
        {
            if (rank <= 0)
                throw new ArgumentOutOfRangeException(nameof(rank), rank, "User rank must be greater than zero.");

            if (experience < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(experience),
                    experience,
                    "User experience must not be negative.");

            Rank = rank;
            Experience = experience;
        }
    }
}