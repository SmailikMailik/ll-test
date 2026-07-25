using R3;

namespace LL.User.Core.Progress
{
    internal interface IUserProgress
    {
        int CurrentTotalExperience { get; }
        Observable<int> TotalExperience { get; }
        Observable<int> Rank { get; }

        bool TryAddExperience(int amount);
    }
}