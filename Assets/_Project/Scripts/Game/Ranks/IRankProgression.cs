namespace LL.Game.Ranks
{
    internal interface IRankProgression
    {
        RankProgress GetProgress(int rank, int experience);
        bool CanPromote(int rank, int experience);
    }
}