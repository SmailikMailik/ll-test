using System;
using LL.Game.Cards;
using LL.Game.Items;
using LL.User.Core.Amounts;
using LL.User.Core.Identity;
using LL.User.Core.Promotions;
using LL.User.Core.Progress;
using LL.User.Core.Rewards;

namespace LL.User.Core
{
    internal sealed class UserInitialData
    {
        internal UserIdentity Identity { get; }
        internal AmountsInitialData<ItemId> Items { get; }
        internal AmountsInitialData<CardId> Cards { get; }
        internal ProgressInitialData Progress { get; }
        internal PromotionOrderInitialData PromotionOrder { get; }
        internal RewardClaimsInitialData RewardClaims { get; }

        internal UserInitialData(
            UserIdentity identity,
            AmountsInitialData<ItemId> items,
            AmountsInitialData<CardId> cards,
            ProgressInitialData progress,
            PromotionOrderInitialData promotionOrder,
            RewardClaimsInitialData rewardClaims)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Cards = cards ?? throw new ArgumentNullException(nameof(cards));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            PromotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            RewardClaims = rewardClaims ?? throw new ArgumentNullException(nameof(rewardClaims));
        }
    }
}