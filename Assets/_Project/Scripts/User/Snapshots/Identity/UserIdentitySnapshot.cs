namespace LL.User.Snapshots.Identity
{
    internal sealed class UserIdentitySnapshot
    {
        private const string UnknownUserId = "0000000";
        private const string UnknownRegionCode = "QQ";

        internal string UserId { get; }
        internal string RegionCode { get; }

        internal UserIdentitySnapshot(string userId, string regionCode)
        {
            UserId = string.IsNullOrWhiteSpace(userId)
                ? UnknownUserId
                : userId.Trim();

            RegionCode = string.IsNullOrWhiteSpace(regionCode)
                ? UnknownRegionCode
                : regionCode.Trim().ToUpperInvariant();
        }
    }
}