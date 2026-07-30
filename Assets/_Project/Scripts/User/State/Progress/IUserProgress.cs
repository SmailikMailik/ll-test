using LL.Game.Ranks;
using LL.User.Snapshots;
using R3;

namespace LL.User.State.Progress
{
    internal interface IUserProgress
    {
        RankId RankId { get; }
        int Rank { get; }
        int Experience { get; }
        bool CanPromoteRank { get; }

        Observable<RankId> RankChanged { get; }
        Observable<int> ExperienceChanged { get; }
        Observable<Unit> Changed { get; }

        UserProgressSnapshot CreateSnapshot();
        int GetApplicableExperience(int amount);
        bool CanAddExperience(int amount);
        bool TryAddExperience(int amount);
        bool TryPromoteRank();
    }
}