using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Currencies;
using LL.User.Core;
using LL.User.Core.Cards;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
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
                        new CardStack(new CardId(card.Id), card.Amount))));
        }

        internal static UserSaveData ToSaveData(UserInitialData data, int rank)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ToSaveData(
                data.Identity,
                new ProgressSaveData(rank, data.Progress.TotalExperience),
                data.Cards.Stacks,
                data.Wallet.Balances);
        }

        internal static UserSaveData ToSaveData(
            UserIdentity identity,
            ProgressSaveData progress,
            IEnumerable<CardStack> cards,
            IEnumerable<CurrencyBalance> balances)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            if (balances == null)
                throw new ArgumentNullException(nameof(balances));

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
                    .ToArray());
        }
    }
}