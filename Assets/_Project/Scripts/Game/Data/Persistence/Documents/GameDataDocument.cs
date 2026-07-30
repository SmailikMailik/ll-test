using System;

namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class GameDataDocument
    {
        internal const int CurrentVersion = 1;

        public int Version { get; }
        public RankDocumentEntry[] Ranks { get; }
        public CardDocumentEntry[] Cards { get; }
        public QuestDocumentEntry[] Quests { get; }
        public RankUpDocumentEntry[] RankUps { get; }
        public RewardDocumentEntry[] Rewards { get; }

        public GameDataDocument(
            int version,
            RankDocumentEntry[] ranks,
            CardDocumentEntry[] cards,
            QuestDocumentEntry[] quests,
            RankUpDocumentEntry[] rankUps,
            RewardDocumentEntry[] rewards)
        {
            Version = version;
            Ranks = ranks ?? Array.Empty<RankDocumentEntry>();
            Cards = cards ?? Array.Empty<CardDocumentEntry>();
            Quests = quests ?? Array.Empty<QuestDocumentEntry>();
            RankUps = rankUps ?? Array.Empty<RankUpDocumentEntry>();
            Rewards = rewards ?? Array.Empty<RewardDocumentEntry>();
        }
    }
}