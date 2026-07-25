using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Currencies;
using LL.Game.ExperienceCards;
using LL.User.Core;
using LL.User.Core.ExperienceCards;
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
                        new CurrencyBalance(
                            new CurrencyId(currency.Id),
                            currency.Amount))),
                new ProgressInitialData(data.TotalExperience),
                new ExperienceCardsInitialData(
                    data.ExperienceCards.Select(card =>
                        new ExperienceCardStack(
                            new ExperienceCardId(card.Id),
                            card.Amount))));
        }

        internal static UserSaveData ToSaveData(UserInitialData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return ToSaveData(
                data.Identity,
                data.Wallet.Balances,
                data.Progress.TotalExperience,
                data.ExperienceCards);
        }

        internal static UserSaveData ToSaveData(
            UserIdentity identity,
            IEnumerable<CurrencyBalance> balances,
            int totalExperience,
            ExperienceCardsInitialData experienceCards)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (balances == null)
                throw new ArgumentNullException(nameof(balances));

            if (experienceCards == null)
                throw new ArgumentNullException(nameof(experienceCards));

            return new UserSaveData(
                identity.RegionCode,
                identity.UserId,
                balances
                    .Where(balance => balance != null)
                    .Select(balance => new CurrencySaveData(
                        balance.Id.Value,
                        balance.Amount))
                    .ToArray(),
                totalExperience,
                experienceCards.Stacks
                    .Select(stack => new ExperienceCardSaveData(
                        stack.Id.Value,
                        stack.Amount))
                    .ToArray());
        }
    }
}