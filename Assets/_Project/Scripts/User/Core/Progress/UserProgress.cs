using System;
using LL.Game.Ranks;
using R3;
using VContainer;

namespace LL.User.Core.Progress
{
    internal sealed class UserProgress : IUserProgress, IDisposable
    {
        public int Rank { get; private set; }
        public int Experience { get; private set; }
        public bool CanPromoteRank => _rankProgression.CanPromote(Rank, Experience);

        public Observable<int> RankChanged => _rankChanged;
        public Observable<int> ExperienceChanged => _experienceChanged;

        private readonly Subject<int> _rankChanged = new();
        private readonly Subject<int> _experienceChanged = new();
        private readonly IRankProgression _rankProgression;

        [Inject]
        internal UserProgress(
            ProgressInitialData initialData,
            IRankProgression rankProgression)
        {
            if (initialData == null)
                throw new ArgumentNullException(nameof(initialData));

            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));

            var progress = _rankProgression.GetProgress(initialData.Rank, initialData.Experience);

            Rank = progress.Rank;
            Experience = progress.HasNextRank
                ? Math.Min(progress.Experience, progress.RequiredExperience)
                : 0;
        }

        public int GetApplicableExperience(int amount)
        {
            if (amount <= 0)
                return 0;

            var progress = _rankProgression.GetProgress(Rank, Experience);
            return Math.Min(amount, progress.RemainingExperience);
        }

        public bool CanAddExperience(int amount) => GetApplicableExperience(amount) > 0;

        public bool TryAddExperience(int amount)
        {
            var appliedExperience = GetApplicableExperience(amount);

            if (appliedExperience <= 0)
                return false;

            Experience += appliedExperience;
            _experienceChanged.OnNext(Experience);

            return true;
        }

        public bool TryPromoteRank()
        {
            if (CanPromoteRank is false)
                return false;

            Rank++;
            Experience = 0;

            _rankChanged.OnNext(Rank);
            _experienceChanged.OnNext(Experience);

            return true;
        }

        public void Dispose()
        {
            _rankChanged.Dispose();
            _experienceChanged.Dispose();
        }
    }
}