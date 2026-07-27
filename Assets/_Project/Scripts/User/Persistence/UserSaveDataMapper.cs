using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Currencies;
using LL.Game.Cards;
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
                new UserIdentity(data.RegionCode, data.UserId),
                new WalletInitialData(
                    data.Currencies.Select(currency =>
                        new CurrencyBalance(new CurrencyId(currency.Id), currency.Amount))),
                new ProgressInitialData(data.Rank, data.TotalExperience),
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
                data.Wallet.Balances,
                rank,
                data.Progress.TotalExperience,
                data.Cards.Stacks);
        }

        internal static UserSaveData ToSaveData(
            UserIdentity identity,
            IEnumerable<CurrencyBalance> balances,
            int rank,
            int totalExperience,
            IEnumerable<CardStack> cards)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (balances == null)
                throw new ArgumentNullException(nameof(balances));

            if (cards == null)
                throw new ArgumentNullException(nameof(cards));

            return new UserSaveData(
                identity.RegionCode,
                identity.UserId,
                balances
                    .Where(balance => balance != null)
                    .Select(balance => new CurrencySaveData(balance.Id.Value, balance.Amount))
                    .ToArray(),
                cards
                    .Where(stack => stack != null)
                    .Select(stack => new CardSaveData(stack.Id.Value, stack.Amount))
                    .ToArray(),
                rank,
                totalExperience);
        }
    }
}