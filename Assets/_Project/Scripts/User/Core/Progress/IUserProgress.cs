using R3;

namespace LL.User.Core.Progress
{
    internal interface IUserProgress
    {
        int Rank { get; }
        int TotalExperience { get; }
        bool CanPromoteRank { get; }

        Observable<int> RankChanged { get; }
        Observable<int> TotalExperienceChanged { get; }

        bool CanAddExperience(int amount);
        bool TryAddExperience(int amount);
        bool TryPromoteRank();
    }
}