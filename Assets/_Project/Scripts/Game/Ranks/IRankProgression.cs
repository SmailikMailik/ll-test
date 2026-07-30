namespace LL.Game.Ranks
{
    internal interface IRankProgression
    {
        RankProgress GetProgress(RankId rankId, int experience);
        bool CanRankUp(RankId rankId, int experience);
    }
}