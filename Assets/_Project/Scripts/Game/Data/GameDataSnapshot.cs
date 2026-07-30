using System;
using LL.Game.Cards;
using LL.Game.Promotions;
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
        internal RankPromotionCatalog RankPromotions { get; }
        internal RewardCatalog Rewards { get; }

        internal GameDataSnapshot(
            RankCatalog ranks,
            CardCatalog cards,
            QuestCatalog quests,
            RankPromotionCatalog rankPromotions,
            RewardCatalog rewards)
        {
            Ranks = ranks ?? throw new ArgumentNullException(nameof(ranks));
            Cards = cards ?? throw new ArgumentNullException(nameof(cards));
            Quests = quests ?? throw new ArgumentNullException(nameof(quests));
            RankPromotions = rankPromotions ?? throw new ArgumentNullException(nameof(rankPromotions));
            Rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
        }
    }
}