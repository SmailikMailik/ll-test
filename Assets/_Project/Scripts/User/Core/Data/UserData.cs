using System;

namespace LL.User.Core.Data
{
    internal sealed class UserData
    {
        internal int SoftAmount { get; }
        internal int HardAmount { get; }
        internal int MasterPointAmount { get; }
        internal int TotalExperience { get; }

        internal UserData(
            int softAmount,
            int hardAmount,
            int masterPointAmount,
            int totalExperience)
        {
            SoftAmount = Math.Max(0, softAmount);
            HardAmount = Math.Max(0, hardAmount);
            MasterPointAmount = Math.Max(0, masterPointAmount);
            TotalExperience = Math.Max(0, totalExperience);
        }
    }
}