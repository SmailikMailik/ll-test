namespace LL.User.Core.Identity
{
    internal sealed class UserIdentity
    {
        private const string UnknownRegionCode = "QQ";
        private const string UnknownUserId = "0000000";

        internal string RegionCode { get; }
        internal string UserId { get; }

        internal UserIdentity(string regionCode, string userId)
        {
            RegionCode = string.IsNullOrWhiteSpace(regionCode)
                ? UnknownRegionCode
                : regionCode.Trim().ToUpperInvariant();

            UserId = string.IsNullOrWhiteSpace(userId)
                ? UnknownUserId
                : userId.Trim();
        }
    }
}