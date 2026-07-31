using System;
using LL.Validation;

namespace LL.User.Snapshots
{
    internal sealed class UserIdentitySnapshot
    {
        internal string UserId { get; }
        internal string RegionCode { get; }

        internal UserIdentitySnapshot(string userId, string regionCode)
        {
            if (ValidationChecks.IsEmpty(userId))
                throw new ArgumentException("User ID must be non-empty.", nameof(userId));

            if (ValidationChecks.IsNotTrimmed(userId))
                throw new ArgumentException(
                    "User ID must not contain leading or trailing whitespace.",
                    nameof(userId));

            if (ValidationChecks.IsEmpty(regionCode))
                throw new ArgumentException("Region code must be non-empty.", nameof(regionCode));

            if (ValidationChecks.IsNotTrimmed(regionCode))
                throw new ArgumentException(
                    "Region code must not contain leading or trailing whitespace.",
                    nameof(regionCode));

            if (ValidationChecks.IsNotUppercase(regionCode))
                throw new ArgumentException("Region code must be uppercase.", nameof(regionCode));

            UserId = userId;
            RegionCode = regionCode;
        }
    }
}