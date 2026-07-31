using System.Collections.Generic;
using LL.Infrastructure.Collections;

namespace LL.Game.Data.Declarations
{
    internal sealed class GameDataDeclaration
    {
        internal IReadOnlyList<RankDeclaration> Ranks { get; }
        internal IReadOnlyList<CardDeclaration> Cards { get; }
        internal IReadOnlyList<HeroDeclaration> Heroes { get; }
        internal IReadOnlyList<QuestDeclaration> Quests { get; }
        internal IReadOnlyList<RankUpDeclaration> RankUps { get; }
        internal IReadOnlyList<RewardDeclaration> Rewards { get; }

        internal GameDataDeclaration(
            IEnumerable<RankDeclaration> ranks,
            IEnumerable<CardDeclaration> cards,
            IEnumerable<HeroDeclaration> heroes,
            IEnumerable<QuestDeclaration> quests,
            IEnumerable<RankUpDeclaration> rankUps,
            IEnumerable<RewardDeclaration> rewards)
        {
            Ranks = ranks.ToReadOnlyCopy();
            Cards = cards.ToReadOnlyCopy();
            Heroes = heroes.ToReadOnlyCopy();
            Quests = quests.ToReadOnlyCopy();
            RankUps = rankUps.ToReadOnlyCopy();
            Rewards = rewards.ToReadOnlyCopy();
        }
    }
}