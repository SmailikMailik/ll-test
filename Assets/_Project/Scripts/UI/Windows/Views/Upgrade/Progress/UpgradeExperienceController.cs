using System;
using LL.Game.Ranks;
using LL.UI.Formatting;
using LL.User.Core.Progress;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade.Progress
{
    [DisallowMultipleComponent]
    internal sealed class UpgradeExperienceController : MonoBehaviour
    {
        [SerializeField] private UpgradeExperienceView _view;

        private const int MinimumAmount = 0;

        internal bool CanApplyPendingExperience =>
            _pendingExperience > MinimumAmount &&
            _userProgress.CurrentTotalExperience <= int.MaxValue - _pendingExperience;

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;
        private int _baseExperience;
        private int _pendingExperience;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression)
        {
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankProgression = rankProgression ?? throw new ArgumentNullException(nameof(rankProgression));
        }

        internal void ResetPreview()
        {
            _baseExperience = _userProgress.CurrentTotalExperience;
            ClearPreview();
        }

        internal int GetMaximumApplicableAmount(int experiencePerItem)
        {
            if (experiencePerItem <= 0)
                return MinimumAmount;

            return (int)Math.Min(
                int.MaxValue,
                ((long)int.MaxValue - _baseExperience) / experiencePerItem);
        }

        internal void SetPendingItems(int amount, int experiencePerItem)
        {
            if (amount < MinimumAmount)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (experiencePerItem <= 0)
                throw new ArgumentOutOfRangeException(nameof(experiencePerItem));

            var pendingExperience = (long)amount * experiencePerItem;

            if ((long)_baseExperience + pendingExperience > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(amount));

            _pendingExperience = (int)pendingExperience;
            RefreshView();
        }

        internal void ClearPreview()
        {
            _pendingExperience = MinimumAmount;
            RefreshView();
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
            var experience = progress.HasNextRank
                ? TextFormatter.Progress(progress.TotalExperience, progress.NextRankExperience)
                : TextFormatter.Number(progress.TotalExperience);
            var addedExperience = _pendingExperience > MinimumAmount
                ? $"+ {TextFormatter.Number(_pendingExperience)}"
                : string.Empty;

            _view.Show(
                progress.Rank.ToString(),
                experience,
                addedExperience,
                progress.GetNormalizedExperience(_baseExperience),
                progress.NormalizedExperience);
        }
    }
}