using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Currencies;
using LL.Game.Items;
using LL.Rewards;
using LL.User.Core;
using LL.User.Core.Cards;
using LL.User.Core.Identity;
using LL.User.Core.Items;
using LL.User.Core.Progress;
using LL.User.Core.Rewards;
using LL.User.Core.Wallet;

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
                new WalletInitialData(
                    data.Currencies.Select(currency =>
                        new CurrencyBalance(new CurrencyId(currency.Id), currency.Amount))),
                new ProgressInitialData(data.Progress.Rank, data.Progress.TotalExperience),
                new CardsInitialData(
                    data.Cards.Select(card =>
                        new CardStack(new CardId(card.Id), card.Amount))),
                new ItemsInitialData(
                    data.Items.Select(item =>
                        new ItemStack(new ItemId(item.Id), item.Amount))),
                new RewardClaimsInitialData(
                    data.ClaimedRewardIds.Select(id => new RewardBundleId(id))));
        }

        internal static UserSaveData ToSaveData(UserInitialData data, int rank)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ToSaveData(
                data.Identity,
                new ProgressSaveData(rank, data.Progress.TotalExperience),
                data.Cards.Stacks,
                data.Wallet.Balances,
                data.Items.Stacks,
                data.RewardClaims.ClaimedIds);
        }

        internal static UserSaveData ToSaveData(
            UserIdentity identity,
            ProgressSaveData progress,
            IEnumerable<CardStack> cards,
            IEnumerable<CurrencyBalance> balances,
            IEnumerable<ItemStack> items,
            IEnumerable<RewardBundleId> claimedRewardIds)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (balances == null)
                throw new ArgumentNullException(nameof(balances));

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
                balances
                    .Where(balance => balance != null)
                    .Select(balance => new CurrencySaveData(balance.Id.Value, balance.Amount))
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