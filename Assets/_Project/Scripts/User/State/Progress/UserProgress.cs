using System;
using LL.Game.Ranks;
using LL.User.Snapshots;
using R3;
using VContainer;

namespace LL.User.State.Progress
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
            UserProgressSnapshot snapshot,
            IRankProgression rankProgression)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));

            var progress = _rankProgression.GetProgress(snapshot.Rank, snapshot.Experience);

            Rank = progress.Rank;
            Experience = progress.Experience;
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