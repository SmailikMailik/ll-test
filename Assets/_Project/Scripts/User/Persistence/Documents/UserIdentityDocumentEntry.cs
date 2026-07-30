namespace LL.User.Persistence.Documents
{
    internal sealed class UserIdentityDocumentEntry
    {
        public string UserId { get; }
        public string RegionCode { get; }

        public UserIdentityDocumentEntry(string userId, string regionCode)
        {
            UserId = userId;
            RegionCode = regionCode;
        }
    }
}