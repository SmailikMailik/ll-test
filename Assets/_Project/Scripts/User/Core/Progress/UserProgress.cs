using System;
using LL.User.Core.Data;
using R3;

namespace LL.User.Core.Progress
{
    internal sealed class UserProgress : IUserProgress, IDisposable
    {
        public Observable<int> TotalExperience => _totalExperience;
        public Observable<int> Level => _level;

        private readonly ReactiveProperty<int> _totalExperience;
        private readonly Observable<int> _level;

        internal UserProgress(UserData initialData, ILevelProgression levelProgression)
        {
            _totalExperience = new ReactiveProperty<int>(initialData.TotalExperience);
            _level = _totalExperience
                .Select(levelProgression.GetLevel)
                .DistinctUntilChanged();
        }

        public bool TryAddExperience(int amount)
        {
            if (amount <= 0 || _totalExperience.Value > int.MaxValue - amount)
                return false;

            _totalExperience.Value += amount;
            return true;
        }

        public void Dispose()
        {
            _totalExperience.Dispose();
        }
    }
}