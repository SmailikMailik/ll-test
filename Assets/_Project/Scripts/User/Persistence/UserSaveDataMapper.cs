using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Rewards;
using LL.User.Core;
using LL.User.Core.Cards;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
using LL.User.Core.Items;
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
                new UserIdentity(data.Identity.RegionCode, data.Identity.UserId),
                new ItemsInitialData(
                    data.Items.Select(item =>
                        new ItemAmount(new ItemId(item.Id), item.Amount))),
                new ProgressInitialData(data.Progress.Rank, data.Progress.Experience),
                new CardsInitialData(
                    data.Cards.Select(card =>
                        new CardStack(new CardId(card.Id), card.Amount))),
                new RewardClaimsInitialData(
                    data.ClaimedRewardIds.Select(id => new RewardBundleId(id))));
        }

        internal static UserSaveData ToSaveData(UserInitialData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ToSaveData(
                data.Identity,
                new ProgressSaveData(data.Progress.Rank, data.Progress.Experience),
                data.Cards.Stacks,
                data.Items.Amounts,
                data.RewardClaims.ClaimedIds);
        }

        internal static UserSaveData ToSaveData(
            UserIdentity identity,
            ProgressSaveData progress,
            IEnumerable<CardStack> cards,
            IEnumerable<ItemAmount> items,
            IEnumerable<RewardBundleId> claimedRewardIds)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (claimedRewardIds == null)
                throw new ArgumentNullException(nameof(claimedRewardIds));

            return new UserSaveData(
                new IdentitySaveData(identity.RegionCode, identity.UserId),
                progress,
                cards
                    .Where(stack => stack != null)
                    .Select(stack => new CardSaveData(stack.Id.Value, stack.Amount))
                    .ToArray(),
                items
                    .Where(item => item != null)
                    .Select(item => new ItemSaveData(item.Id.Value, item.Amount))
                    .ToArray(),
                claimedRewardIds
                    .Where(id => id.IsEmpty is false)
                    .Select(id => id.Value)
                    .ToArray());
        }
    }
}