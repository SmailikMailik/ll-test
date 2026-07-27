namespace LL.Game.Ranks
{
    internal interface IRankProgression
    {
        int GetRank(int totalExperience);
        RankProgress GetProgress(int rank, int totalExperience);
        bool CanPromote(int rank, int totalExperience);
    }
}