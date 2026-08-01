namespace LL.User.Defaults.Declarations
{
    internal sealed class UserIdentityDefaultDeclaration
    {
        internal string UserId { get; }
        internal string RegionCode { get; }

        internal UserIdentityDefaultDeclaration(string userId, string regionCode)
        {
            UserId = userId;
            RegionCode = regionCode;
        }
    }
}