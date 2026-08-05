using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Heroes;
using LL.Game.Identifiers;
using LL.Game.RankUp;

namespace LL.User.Snapshots
{
    internal sealed class UserHeroSnapshot
    {
        internal HeroId HeroId { get; }
        internal UserProgressSnapshot Progress { get; }
        internal IReadOnlyList<UserRankUpAttemptSnapshot> RankUpAttempts { get; }

        internal UserHeroSnapshot(
            HeroId heroId,
            UserProgressSnapshot progress,
            IEnumerable<UserRankUpAttemptSnapshot> rankUpAttempts)
        {
            IdentifierValidator.EnsureValid(heroId, nameof(heroId));
            HeroId = heroId;
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            RankUpAttempts = Array.AsReadOnly(rankUpAttempts?.ToArray() ?? Array.Empty<UserRankUpAttemptSnapshot>());

            var optionIds = new HashSet<RankUpOptionId>();

            if (RankUpAttempts.Any(attempt => attempt is null || optionIds.Add(attempt.OptionId) is false))
                throw new ArgumentException("Rank-up attempts must be non-null and unique by option.", nameof(rankUpAttempts));
        }
    }
}