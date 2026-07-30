using System;
using System.Collections.Generic;

namespace LL.Game.Data.Declarations
{
    internal sealed class GameDataDeclaration
    {
        internal IReadOnlyList<RankDeclaration> Ranks { get; }
        internal IReadOnlyList<CardDeclaration> Cards { get; }
        internal IReadOnlyList<QuestDeclaration> Quests { get; }
        internal IReadOnlyList<RankUpDeclaration> RankUps { get; }
        internal IReadOnlyList<RewardDeclaration> Rewards { get; }

        internal GameDataDeclaration(
            IEnumerable<RankDeclaration> ranks,
            IEnumerable<CardDeclaration> cards,
            IEnumerable<QuestDeclaration> quests,
            IEnumerable<RankUpDeclaration> rankUps,
            IEnumerable<RewardDeclaration> rewards)
        {
            Ranks = Copy(ranks);
            Cards = Copy(cards);
            Quests = Copy(quests);
            RankUps = Copy(rankUps);
            Rewards = Copy(rewards);
        }

        private static IReadOnlyList<T> Copy<T>(IEnumerable<T> entries)
        {
            return Array.AsReadOnly(entries == null ? Array.Empty<T>() : new List<T>(entries).ToArray());
        }
    }
}