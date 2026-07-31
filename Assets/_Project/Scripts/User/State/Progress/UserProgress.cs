using System;
using LL.Game.Ranks;
using LL.User.Snapshots;
using R3;
using VContainer;

namespace LL.User.State.Progress
{
    internal sealed class UserProgress : IUserProgress, IUserProgressCommands, IDisposable
    {
        public RankId RankId { get; private set; }
        public int Rank { get; private set; }
        public int Experience { get; private set; }
        public bool CanRankUp => _rankProgression.CanRankUp(RankId, Experience);

        public Observable<Unit> Changed => _changed;

        private readonly Subject<Unit> _changed = new();
        private readonly IRankProgression _rankProgression;

        [Inject]
        internal UserProgress(
            UserProgressSnapshot snapshot,
            IRankProgression rankProgression)
        {
            if (snapshot == null)
                throw new ArgumentNullException(nameof(snapshot));

            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));

            var progress = _rankProgression.GetProgress(snapshot.RankId, snapshot.Experience);

            RankId = progress.RankId;
            Rank = progress.Rank;
            Experience = progress.Experience;
        }

        public int GetApplicableExperience(int amount)
        {
            if (amount <= 0)
                return 0;

            var progress = _rankProgression.GetProgress(RankId, Experience);
            return Math.Min(amount, progress.RemainingExperience);
        }

        public bool CanAddExperience(int amount) => GetApplicableExperience(amount) > 0;

        public UserProgressSnapshot CreateSnapshot() => new(RankId, Experience);

        public bool TryAddExperience(int amount)
        {
            var appliedExperience = GetApplicableExperience(amount);

            if (appliedExperience <= 0)
                return false;

            Experience += appliedExperience;
            _changed.OnNext(Unit.Default);

            return true;
        }

        public bool TryRankUp()
        {
            if (CanRankUp is false)
                return false;

            var progress = _rankProgression.GetProgress(RankId, Experience);
            var nextProgress = _rankProgression.GetProgress(progress.NextRankId, 0);

            RankId = nextProgress.RankId;
            Rank = nextProgress.Rank;
            Experience = 0;

            _changed.OnNext(Unit.Default);

            return true;
        }

        public void Dispose()
        {
            _changed.Dispose();
        }
    }
}