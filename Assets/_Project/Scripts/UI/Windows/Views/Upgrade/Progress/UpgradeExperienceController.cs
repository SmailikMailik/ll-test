using System;
using LL.Game.Ranks;
using LL.User.Core.Progress;

namespace LL.UI.Windows.Views.Upgrade.Progress
{
    internal sealed class UpgradeExperienceController
    {
        private const int MinimumAmount = 0;

        internal bool CanApplyPendingExperience => _userProgress.CanAddExperience(_pendingExperience);

        private readonly UpgradeExperienceView _view;
        private readonly IUserProgress _userProgress;
        private readonly IRankProgression _rankProgression;

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
            _baseExperience = _userProgress.CurrentTotalExperience;
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

            if (amount > MinimumAmount &&
                _userProgress.CanAddExperience(amount) is false)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

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

            var totalAvailableExperience = int.MaxValue - _baseExperience;

            if (reservedExperience >= totalAvailableExperience)
                return MinimumAmount;

            var availableExperience = totalAvailableExperience - reservedExperience;
            return availableExperience / experiencePerItem;
        }

        internal bool TryApplyPendingExperience()
        {
            if (CanApplyPendingExperience is false ||
                _userProgress.TryAddExperience(_pendingExperience) is false)
            {
                return false;
            }

            ResetPreview();
            return true;
        }

        private void RefreshView()
        {
            var previewExperience = _baseExperience + _pendingExperience;
            var progress = _rankProgression.GetProgress(previewExperience);

            _view.Show(progress, _baseExperience, _pendingExperience);
        }
    }
}