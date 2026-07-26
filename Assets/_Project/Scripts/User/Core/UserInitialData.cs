using System;
using LL.User.Core.Cards;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
using LL.User.Core.Wallet;

namespace LL.User.Core
{
    internal sealed class UserInitialData
    {
        internal UserIdentity Identity { get; }
        internal WalletInitialData Wallet { get; }
        internal ProgressInitialData Progress { get; }
        internal CardsInitialData Cards { get; }

        internal UserInitialData(
            UserIdentity identity,
            WalletInitialData wallet,
            ProgressInitialData progress,
            CardsInitialData cards)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            Cards = cards ?? throw new ArgumentNullException(nameof(cards));
        }
    }
}