using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Game.Promotions;
using LL.Rewards.Models;
using LL.User.Core;
using LL.User.Core.Amounts;
using LL.User.Core.Identity;
using LL.User.Core.Promotions;
using LL.User.Core.Progress;
using LL.User.Core.Rewards;

namespace LL.User.Persistence
{
    internal static class UserSaveDataMapper
    {
        internal static UserInitialData ToInitialData(UserSaveData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return new UserInitialData(
                new UserIdentity(data.Identity.UserId, data.Identity.RegionCode),
                new AmountsInitialData<ItemId>(
                    data.Items.Select(item =>
                        new Amount<ItemId>(new ItemId(item.Id), item.Amount))),
                new AmountsInitialData<CardId>(
                    data.Cards.Select(card =>
                        new Amount<CardId>(new CardId(card.Id), card.Amount))),
                new ProgressInitialData(data.Progress.Rank, data.Progress.Experience),
                new PromotionOrderInitialData(
                    new PromotionRequirementId(data.PromotionOrder.RequirementId),
                    data.PromotionOrder.DeadlineUnixMilliseconds,
                    data.PromotionOrder.IsCompleted),
                new RewardClaimsInitialData(
                    data.ClaimedRewardIds.Select(id => new RewardBundleId(id))));
        }

        internal static UserSaveData ToSaveData(UserInitialData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ToSaveData(
                data.Identity,
                data.Progress,
                data.Items.Amounts,
                data.Cards.Amounts,
                data.PromotionOrder,
                data.RewardClaims.ClaimedIds);
        }

        internal static UserSaveData ToSaveData(
            UserIdentity identity,
            ProgressInitialData progress,
            IEnumerable<Amount<ItemId>> items,
            IEnumerable<Amount<CardId>> cards,
            PromotionOrderInitialData promotionOrder,
            IEnumerable<RewardBundleId> claimedRewardIds)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (promotionOrder == null)
                throw new ArgumentNullException(nameof(promotionOrder));

            if (claimedRewardIds == null)
                throw new ArgumentNullException(nameof(claimedRewardIds));

            return new UserSaveData(
                new IdentitySaveData(identity.UserId, identity.RegionCode),
                new ProgressSaveData(progress.Rank, progress.Experience),
                new PromotionOrderSaveData(
                    promotionOrder.RequirementId.Value,
                    promotionOrder.DeadlineUnixMilliseconds,
                    promotionOrder.IsCompleted),
                items
                    .Select(item => new AmountSaveData(item.Id.Value, item.Value))
                    .ToArray(),
                cards
                    .Select(card => new AmountSaveData(card.Id.Value, card.Value))
                    .ToArray(),
                claimedRewardIds
                    .Where(id => id.IsEmpty is false)
                    .Select(id => id.Value)
                    .ToArray());
        }
    }
}