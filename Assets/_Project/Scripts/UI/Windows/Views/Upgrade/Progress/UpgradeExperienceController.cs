using System;
using LL.Game.Ranks;
using LL.User.State.Progress;

namespace LL.UI.Windows.Views.Upgrade.Progress
{
    internal sealed class UpgradeExperienceController
    {
        private const int MinimumAmount = 0;

        internal int RemainingExperience
        {
            get
            {
                var progress = _rankProgression.GetProgress(_baseRank, _baseExperience);
                return progress.RemainingExperience;
            }
        }

        internal bool CanApplyPendingExperience =>
            _pendingExperience > MinimumAmount &&
            _userProgress.CanAddExperience(_pendingExperience);

        private readonly UpgradeExperienceView _view;
        private readonly IUserProgress _userProgress;
        private readonly IRankProgression _rankProgression;

        private int _baseRank;
        private int _baseExperience;
        private int _pendingExperience;

        internal UpgradeExperienceController(
            UpgradeExperienceView view,
            IUserProgress userProgress,
            IRankProgression rankProgression)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
        }

        internal void ResetPreview()
        {
            _baseRank = _userProgress.Rank;
            _baseExperience = _userProgress.Experience;
            ClearPreview();
        }

        internal void ClearPreview()
        {
            _pendingExperience = MinimumAmount;
            RefreshView();
        }

        internal void SetPendingExperience(int amount)
        {
            if (amount < MinimumAmount)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (amount > MinimumAmount && _userProgress.CanAddExperience(amount) is false)
                throw new ArgumentOutOfRangeException(nameof(amount));

            _pendingExperience = amount;
            RefreshView();
        }

        internal int GetMaximumApplicableAmount(
            int experiencePerItem,
            int reservedExperience)
        {
            if (experiencePerItem <= 0)
                return MinimumAmount;

            if (reservedExperience < MinimumAmount)
                throw new ArgumentOutOfRangeException(nameof(reservedExperience));

            if (reservedExperience >= RemainingExperience)
                return MinimumAmount;

            var requiredExperience = RemainingExperience - reservedExperience;
            return (requiredExperience - 1) / experiencePerItem + 1;
        }

        private void RefreshView()
        {
            var progress = _rankProgression.GetProgress(_baseRank, _baseExperience);
            _view.Show(progress, _baseExperience, _pendingExperience);
        }
    }
}