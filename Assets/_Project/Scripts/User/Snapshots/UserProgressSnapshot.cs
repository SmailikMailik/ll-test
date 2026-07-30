using System;
using LL.Game.Identifiers;
using LL.Game.Ranks;

namespace LL.User.Snapshots
{
    internal sealed class UserProgressSnapshot
    {
        internal RankId RankId { get; }
        internal int Experience { get; }

        internal UserProgressSnapshot(RankId rankId, int experience)
        {
            if (experience < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(experience),
                    experience,
                    "User experience must not be negative.");

            IdentifierValidator.EnsureValid(rankId, nameof(rankId));

            RankId = rankId;
            Experience = experience;
        }
    }
}