using System;
using LL.Game.Cards;
using LL.Game.RankUp;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;

namespace LL.Game.Data
{
    internal sealed class GameDataSnapshot
    {
        internal RankCatalog Ranks { get; }
        internal CardCatalog Cards { get; }
        internal QuestCatalog Quests { get; }
        internal RankUpCatalog RankUps { get; }
        internal RewardCatalog Rewards { get; }

        internal GameDataSnapshot(
            RankCatalog ranks,
            CardCatalog cards,
            QuestCatalog quests,
            RankUpCatalog rankUps,
            RewardCatalog rewards)
        {
            Ranks = ranks ?? throw new ArgumentNullException(nameof(ranks));
            Cards = cards ?? throw new ArgumentNullException(nameof(cards));
            Quests = quests ?? throw new ArgumentNullException(nameof(quests));
            RankUps = rankUps ?? throw new ArgumentNullException(nameof(rankUps));
            Rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
        }
    }
}