using R3;

namespace LL.User.Core.Progress
{
    internal interface IUserProgress
    {
        int Rank { get; }
        int Experience { get; }
        bool CanPromoteRank { get; }

        Observable<int> RankChanged { get; }
        Observable<int> ExperienceChanged { get; }

        int GetApplicableExperience(int amount);
        bool CanAddExperience(int amount);
        bool TryAddExperience(int amount);
        bool TryPromoteRank();
    }
}