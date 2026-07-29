using System;

namespace LL.User.Snapshots
{
    internal sealed class UserIdentitySnapshot
    {
        internal string UserId { get; }
        internal string RegionCode { get; }

        internal UserIdentitySnapshot(string userId, string regionCode)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID must be non-empty.", nameof(userId));

            if (userId != userId.Trim())
                throw new ArgumentException(
                    "User ID must not contain leading or trailing whitespace.",
                    nameof(userId));

            if (string.IsNullOrWhiteSpace(regionCode))
                throw new ArgumentException("Region code must be non-empty.", nameof(regionCode));

            if (regionCode != regionCode.Trim())
                throw new ArgumentException(
                    "Region code must not contain leading or trailing whitespace.",
                    nameof(regionCode));

            if (regionCode != regionCode.ToUpperInvariant())
                throw new ArgumentException("Region code must be uppercase.", nameof(regionCode));

            UserId = userId;
            RegionCode = regionCode;
        }
    }
}