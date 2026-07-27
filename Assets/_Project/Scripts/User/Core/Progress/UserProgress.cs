using System;
using R3;
using VContainer;

namespace LL.User.Core.Progress
{
    internal sealed class UserProgress : IUserProgress, IDisposable
    {
        public int TotalExperience { get; private set; }

        public Observable<int> TotalExperienceChanged => _totalExperienceChanged;

        private readonly Subject<int> _totalExperienceChanged = new();

        [Inject]
        internal UserProgress(ProgressInitialData initialData)
        {
            TotalExperience = initialData.TotalExperience;
        }

        public bool CanAddExperience(int amount)
        {
            if (amount <= 0)
                return false;

            var availableExperience = int.MaxValue - TotalExperience;
            return amount <= availableExperience;
        }

        public bool TryAddExperience(int amount)
        {
            if (CanAddExperience(amount) is false)
                return false;

            TotalExperience += amount;
            _totalExperienceChanged.OnNext(TotalExperience);

            return true;
        }

        public void Dispose()
        {
            _totalExperienceChanged.Dispose();
        }
    }
}