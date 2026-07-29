namespace LL.User.Persistence.SaveData
{
    internal sealed class UserIdentitySaveData
    {
        public string UserId { get; }
        public string RegionCode { get; }

        public UserIdentitySaveData(string userId, string regionCode)
        {
            UserId = userId;
            RegionCode = regionCode;
        }
    }
}