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