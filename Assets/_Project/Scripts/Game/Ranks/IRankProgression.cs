namespace LL.Game.Ranks
{
    internal interface IRankProgression
    {
        RankProgress GetProgress(RankId rankId, int experience);
        bool CanPromote(RankId rankId, int experience);
    }
}