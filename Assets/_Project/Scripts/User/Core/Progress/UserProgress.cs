using System;
using LL.Game.Ranks;
using R3;
using VContainer;

namespace LL.User.Core.Progress
{
    internal sealed class UserProgress : IUserProgress, IDisposable
    {
        public int CurrentTotalExperience => _totalExperience.Value;
        public Observable<int> TotalExperience => _totalExperience;
        public Observable<int> Rank => _rank;

        private readonly ReactiveProperty<int> _totalExperience;
        private readonly Observable<int> _rank;

        [Inject]
        internal UserProgress(
            ProgressInitialData initialData,
            IRankProgression rankProgression)
        {
            _totalExperience = new ReactiveProperty<int>(initialData.TotalExperience);
            _rank = _totalExperience
                .Select(rankProgression.GetRank)
                .DistinctUntilChanged();
        }

        public bool CanAddExperience(int amount)
        {
            var hasExperienceToAdd = amount > 0;

            if (hasExperienceToAdd is false)
                return false;

            var currentExperience = _totalExperience.Value;
            var availableExperience = int.MaxValue - currentExperience;

            return amount <= availableExperience;
        }

        public bool TryAddExperience(int amount)
        {
            if (CanAddExperience(amount) is false)
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