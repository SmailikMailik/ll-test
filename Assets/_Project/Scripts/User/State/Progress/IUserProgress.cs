using LL.Game.Ranks;
using LL.User.Snapshots;
using R3;

namespace LL.User.State.Progress
{
    internal interface IUserProgress
    {
        RankId RankId { get; }
        int RankNumber { get; }
        int Experience { get; }
        bool CanRankUp { get; }

        Observable<Unit> Changed { get; }

        UserProgressSnapshot CreateSnapshot();
        int GetApplicableExperience(int amount);
        bool CanAddExperience(int amount);
    }
}