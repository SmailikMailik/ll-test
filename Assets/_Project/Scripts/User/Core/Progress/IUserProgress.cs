using R3;

namespace LL.User.Core.Progress
{
    internal interface IUserProgress
    {
        Observable<int> TotalExperience { get; }
        Observable<int> Level { get; }

        bool TryAddExperience(int amount);
    }
}