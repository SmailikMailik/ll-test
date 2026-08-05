using System;
using LL.Game.Heroes;
using LL.Game.Ranks;
using LL.User.State.Heroes;

namespace LL.UI.Windows.Upgrade.Progress
{
    internal sealed class UpgradeExperienceController
    {
        private const int MinAmount = 0;

        internal int RemainingExperience
        {
            get
            {
                var progress = _rankProgression.GetProgress(_baseRankId, _baseExperience);
                return progress.RemainingExperience;
            }
        }

        internal bool CanApplyPendingExperience =>
            _pendingExperience > MinAmount &&
            _userProgress.CanAddExperience(_heroId, _pendingExperience);

        private readonly UpgradeExperienceView _view;
        private readonly HeroId _heroId;
        private readonly IUserHeroProgress _userProgress;
        private readonly IRankProgression _rankProgression;

        private RankId _baseRankId;
        private int _baseExperience;
        private int _pendingExperience;

        internal UpgradeExperienceController(
            UpgradeExperienceView view,
            HeroId heroId,
            IUserHeroProgress userProgress,
            IRankProgression rankProgression)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _heroId = heroId;
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
        }

        internal void ResetPreview()
        {
            if (_userProgress.TryGetProgress(_heroId, out var progress) is false)
                throw new InvalidOperationException($"Missing user progress for hero '{_heroId}'.");

            _baseRankId = progress.RankId;
            _baseExperience = progress.Experience;
            ClearPreview();
        }

        internal void ClearPreview()
        {
            _pendingExperience = MinAmount;
            RefreshView();
        }

        internal void SetPendingExperience(int amount)
        {
            if (amount < MinAmount)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (amount > MinAmount && _userProgress.CanAddExperience(_heroId, amount) is false)
                throw new ArgumentOutOfRangeException(nameof(amount));

            _pendingExperience = amount;
            RefreshView();
        }

        internal int GetMaxApplicableAmount(
            int experiencePerItem,
            int reservedExperience)
        {
            if (experiencePerItem <= 0)
                return MinAmount;

            if (reservedExperience < MinAmount)
                throw new ArgumentOutOfRangeException(nameof(reservedExperience));

            if (reservedExperience >= RemainingExperience)
                return MinAmount;

            var requiredExperience = RemainingExperience - reservedExperience;
            return (requiredExperience - 1) / experiencePerItem + 1;
        }

        private void RefreshView()
        {
            var progress = _rankProgression.GetProgress(_baseRankId, _baseExperience);
            _view.Show(progress, _baseExperience, _pendingExperience);
        }
    }
}