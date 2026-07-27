using System;
using LL.Game.Ranks;
using R3;
using VContainer;

namespace LL.User.Core.Progress
{
    internal sealed class UserProgress : IUserProgress, IDisposable
    {
        public int Rank { get; private set; }
        public int TotalExperience { get; private set; }
        public bool CanPromoteRank => _rankProgression.CanPromote(Rank, TotalExperience);

        public Observable<int> RankChanged => _rankChanged;
        public Observable<int> TotalExperienceChanged => _totalExperienceChanged;

        private readonly Subject<int> _rankChanged = new();
        private readonly Subject<int> _totalExperienceChanged = new();
        private readonly IRankProgression _rankProgression;

        [Inject]
        internal UserProgress(
            ProgressInitialData initialData,
            IRankProgression rankProgression)
        {
            if (initialData == null)
                throw new ArgumentNullException(nameof(initialData));

            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));

            var initialRank = initialData.ResolveRank(_rankProgression);
            var progress = _rankProgression.GetProgress(initialRank, initialData.TotalExperience);

            Rank = progress.Rank;
            TotalExperience = Math.Max(progress.TotalExperience, progress.CurrentRankExperience);
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

        public bool TryPromoteRank()
        {
            if (CanPromoteRank is false)
                return false;

            Rank++;
            _rankChanged.OnNext(Rank);

            return true;
        }

        public void Dispose()
        {
            _rankChanged.Dispose();
            _totalExperienceChanged.Dispose();
        }
    }
}