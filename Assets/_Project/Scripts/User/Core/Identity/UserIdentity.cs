namespace LL.User.Core.Identity
{
    internal sealed class UserIdentity
    {
        private const string UnknownUserId = "0000000";
        private const string UnknownRegionCode = "QQ";

        internal string UserId { get; }
        internal string RegionCode { get; }

        internal UserIdentity(string userId, string regionCode)
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