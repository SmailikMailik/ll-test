using R3;

namespace LL.User.Core.Progress
{
    internal interface IUserProgress
    {
        int TotalExperience { get; }

        Observable<int> TotalExperienceChanged { get; }

        bool CanAddExperience(int amount);
        bool TryAddExperience(int amount);
    }
}