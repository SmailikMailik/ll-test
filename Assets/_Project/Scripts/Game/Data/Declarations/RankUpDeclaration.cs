using System.Collections.Generic;
using LL.Infrastructure.Collections;

namespace LL.Game.Data.Declarations
{
    internal sealed class RankUpDeclaration
    {
        internal string HeroId { get; }
        internal string RankId { get; }
        internal string RewardId { get; }
        internal IReadOnlyList<RankUpOptionDeclaration> Options { get; }

        internal RankUpDeclaration(
            string heroId,
            string rankId,
            string rewardId,
            IEnumerable<RankUpOptionDeclaration> options)
        {
            HeroId = heroId;
            RankId = rankId;
            RewardId = rewardId;
            Options = options.ToReadOnlyCopy();
        }
    }
}