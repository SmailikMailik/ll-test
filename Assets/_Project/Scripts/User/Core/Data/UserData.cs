using System;

namespace LL.User.Core.Data
{
    internal sealed class UserData
    {
        private const string UnknownRegionCode = "QQ";
        private const string UnknownUserId = "0000000";

        internal string RegionCode { get; }
        internal string UserId { get; }

        internal int SoftAmount { get; }
        internal int HardAmount { get; }
        internal int MasterPointAmount { get; }
        internal int TotalExperience { get; }

        internal UserData(
            string regionCode,
            string userId,
            int softAmount,
            int hardAmount,
            int masterPointAmount,
            int totalExperience)
        {
            RegionCode = string.IsNullOrWhiteSpace(regionCode)
                ? UnknownRegionCode
                : regionCode.Trim().ToUpperInvariant();

            UserId = string.IsNullOrWhiteSpace(userId)
                ? UnknownUserId
                : userId.Trim();

            SoftAmount = Math.Max(0, softAmount);
            HardAmount = Math.Max(0, hardAmount);
            MasterPointAmount = Math.Max(0, masterPointAmount);
            TotalExperience = Math.Max(0, totalExperience);
        }
    }
}