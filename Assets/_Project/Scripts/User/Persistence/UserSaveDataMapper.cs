using System;
using System.Linq;
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
                    data.SoftAmount,
                    data.HardAmount,
                    data.MasterPointAmount),
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
                data.Wallet.SoftAmount,
                data.Wallet.HardAmount,
                data.Wallet.MasterPointAmount,
                data.Progress.TotalExperience,
                data.ExperienceCards);
        }

        internal static UserSaveData ToSaveData(
            UserIdentity identity,
            int softAmount,
            int hardAmount,
            int masterPointAmount,
            int totalExperience,
            ExperienceCardsInitialData experienceCards)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (experienceCards == null)
                throw new ArgumentNullException(nameof(experienceCards));

            return new UserSaveData(
                identity.RegionCode,
                identity.UserId,
                softAmount,
                hardAmount,
                masterPointAmount,
                totalExperience,
                experienceCards.Stacks
                    .Select(stack => new ExperienceCardSaveData(
                        stack.Id.Value,
                        stack.Amount))
                    .ToArray());
        }
    }
}