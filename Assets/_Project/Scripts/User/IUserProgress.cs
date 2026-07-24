using R3;

namespace LL.User
{
    internal interface IUserProgress
    {
        Observable<int> TotalExperience { get; }
        Observable<int> Level { get; }

        bool TryAddExperience(int amount);
    }
}