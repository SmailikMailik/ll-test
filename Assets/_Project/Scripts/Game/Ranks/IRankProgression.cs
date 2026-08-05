namespace LL.Game.Ranks
{
    internal interface IRankProgression
    {
        bool TryGetProgress(RankId rankId, int experience, out RankProgress progress);
        RankProgress GetProgress(RankId rankId, int experience);
        bool CanRankUp(RankId rankId, int experience);
    }
}