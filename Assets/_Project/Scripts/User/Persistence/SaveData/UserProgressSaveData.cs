namespace LL.User.Persistence.SaveData
{
    internal sealed class UserProgressSaveData
    {
        public string RankId { get; }
        public int Experience { get; }

        public UserProgressSaveData(string rankId, int experience)
        {
            RankId = rankId;
            Experience = experience;
        }
    }
}