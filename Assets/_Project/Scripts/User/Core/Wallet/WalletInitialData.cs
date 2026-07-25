using System;

namespace LL.User.Core.Wallet
{
    internal sealed class WalletInitialData
    {
        internal int SoftAmount { get; }
        internal int HardAmount { get; }
        internal int MasterPointAmount { get; }

        internal WalletInitialData(
            int softAmount,
            int hardAmount,
            int masterPointAmount)
        {
            SoftAmount = Math.Max(0, softAmount);
            HardAmount = Math.Max(0, hardAmount);
            MasterPointAmount = Math.Max(0, masterPointAmount);
        }
    }
}