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

        internal void SetPendingItems(int amount, int experiencePerItem)
        {
            if (amount < MinimumAmount)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (experiencePerItem <= 0)
                throw new ArgumentOutOfRangeException(nameof(experiencePerItem));

            var maximumApplicableAmount = GetMaximumApplicableAmount(experiencePerItem);

            if (amount > maximumApplicableAmount)
                throw new ArgumentOutOfRangeException(nameof(amount));

            _pendingExperience = amount * experiencePerItem;
            RefreshView();
        }

        internal int GetMaximumApplicableAmount(int experiencePerItem)
        {
            if (experiencePerItem <= 0)
                return MinimumAmount;

            var availableExperience = int.MaxValue - _baseExperience;
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