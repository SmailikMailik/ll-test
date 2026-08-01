using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RankUpDocumentEntry
    {
        public string HeroId { get; }
        public string RankId { get; }
        public string RewardId { get; }
        public RankUpOptionDocumentEntry[] Options { get; }

        public RankUpDocumentEntry(
            string heroId,
            string rankId,
            string rewardId,
            RankUpOptionDocumentEntry[] options)
        {
            HeroId = heroId;
            RankId = rankId;
            RewardId = rewardId;
            Options = options ?? Array.Empty<RankUpOptionDocumentEntry>();
        }
    }
}