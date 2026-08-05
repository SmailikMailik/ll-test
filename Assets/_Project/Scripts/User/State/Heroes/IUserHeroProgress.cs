using LL.Game.Heroes;
using LL.User.Snapshots;
using R3;

namespace LL.User.State.Heroes
{
    internal interface IUserHeroProgress
    {
        Observable<Unit> Changed { get; }

        bool TryGetProgress(HeroId heroId, out UserProgressSnapshot progress);
        int GetRankNumber(HeroId heroId);
        int GetApplicableExperience(HeroId heroId, int amount);
        bool CanAddExperience(HeroId heroId, int amount);
        bool CanRankUp(HeroId heroId);
    }
}