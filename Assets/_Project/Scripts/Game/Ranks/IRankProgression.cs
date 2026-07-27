namespace LL.Game.Ranks
{
    internal interface IRankProgression
    {
        RankProgress GetProgress(int totalExperience);
    }
}