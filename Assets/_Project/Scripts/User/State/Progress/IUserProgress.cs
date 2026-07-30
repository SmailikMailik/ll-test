using LL.Game.Ranks;
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

        int GetApplicableExperience(int amount);
        bool CanAddExperience(int amount);
        bool TryAddExperience(int amount);
        bool TryPromoteRank();
    }
}