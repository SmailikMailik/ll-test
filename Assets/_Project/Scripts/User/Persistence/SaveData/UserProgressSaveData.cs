namespace LL.User.Persistence.SaveData
{
    internal sealed class UserProgressSaveData
    {
        public int Rank { get; }
        public int Experience { get; }

        public UserProgressSaveData(int rank, int experience)
        {
            Rank = rank;
            Experience = experience;
        }
    }
}