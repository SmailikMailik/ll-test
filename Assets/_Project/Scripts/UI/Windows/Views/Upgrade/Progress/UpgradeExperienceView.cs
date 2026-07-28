using System;
using LL.Game.Ranks;
using LL.UI.Typography;
using TMPro;
using UnityEngine;

namespace LL.UI.Windows.Views.Upgrade.Progress
{
    [DisallowMultipleComponent]
    internal sealed class UpgradeExperienceView : MonoBehaviour
    {
        [SerializeField] private PredictedProgressBar _bar;
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _experienceLabel;
        [SerializeField] private TMP_Text _addedExperienceLabel;
        [SerializeField] private GameObject _maximumExperienceMarker;

        internal void Show(RankProgress progress, int currentExperience, int addedExperience)
        {
            _rankLabel.text = progress.Rank.ToString();

            if (progress.HasNextRank is false)
            {
                ShowMaximumRank(progress, currentExperience);
                return;
            }

            if (addedExperience <= 0)
            {
                ShowCurrentProgress(progress, currentExperience);
                return;
            }

            ShowPreviewProgress(progress, currentExperience, addedExperience);
        }

        private void ShowMaximumRank(RankProgress progress, int currentExperience)
        {
            _experienceLabel.text = TextFormatter.Number(currentExperience);
            _addedExperienceLabel.text = string.Empty;
            _maximumExperienceMarker.SetActive(true);
            SetBarProgress(progress, currentExperience);
        }

        private void ShowCurrentProgress(RankProgress progress, int currentExperience)
        {
            _experienceLabel.text = TextFormatter.Progress(currentExperience, progress.RequiredExperience);
            _addedExperienceLabel.text = string.Empty;
            _maximumExperienceMarker.SetActive(currentExperience >= progress.RequiredExperience);
            SetBarProgress(progress, currentExperience);
        }

        private void ShowPreviewProgress(RankProgress progress, int currentExperience, int addedExperience)
        {
            var previewExperience = Math.Min(currentExperience + addedExperience, progress.RequiredExperience);

            _experienceLabel.text = TextFormatter.Progress(currentExperience, progress.RequiredExperience);
            _addedExperienceLabel.text = $"+ {TextFormatter.Number(addedExperience)}";
            _maximumExperienceMarker.SetActive(previewExperience >= progress.RequiredExperience);
            SetBarProgress(progress, currentExperience, previewExperience);
        }

        private void SetBarProgress(RankProgress progress, int currentExperience)
        {
            SetBarProgress(progress, currentExperience, currentExperience);
        }

        private void SetBarProgress(RankProgress progress, int currentExperience, int previewExperience)
        {
            var currentProgress = progress.GetNormalizedExperience(currentExperience);
            var previewProgress = progress.GetNormalizedExperience(previewExperience);

            _bar.SetProgress(currentProgress, previewProgress);
        }
    }
}