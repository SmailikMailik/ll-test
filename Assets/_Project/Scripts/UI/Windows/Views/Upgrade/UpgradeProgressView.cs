using System;
using LL.Game.Ranks;
using LL.UI.Formatting;
using LL.User.Core.Progress;
using TMPro;
using UnityEngine;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade
{
    [DisallowMultipleComponent]
    internal sealed class UpgradeProgressView : MonoBehaviour
    {
        [SerializeField] private PredictedProgressBar _bar;
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _experienceLabel;
        [SerializeField] private TMP_Text _addedExperienceLabel;

        private const int MinimumAmount = 0;

        internal bool CanApplyPendingExperience =>
            _pendingExperience > 0 &&
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
            Refresh();
        }

        internal void ClearPreview()
        {
            _pendingExperience = 0;
            Refresh();
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

        private void Refresh()
        {
            var previewExperience = _baseExperience + _pendingExperience;
            var progress = _rankProgression.GetProgress(previewExperience);

            _rankLabel.text = progress.Rank.ToString();
            _experienceLabel.text = progress.HasNextRank
                ? TextFormatter.Progress(progress.TotalExperience, progress.NextRankExperience)
                : TextFormatter.Number(progress.TotalExperience);
            _addedExperienceLabel.text = _pendingExperience > 0
                ? $"+ {TextFormatter.Number(_pendingExperience)}"
                : string.Empty;

            _bar.SetProgress(
                progress.GetNormalizedExperience(_baseExperience),
                progress.NormalizedExperience);
        }
    }
}