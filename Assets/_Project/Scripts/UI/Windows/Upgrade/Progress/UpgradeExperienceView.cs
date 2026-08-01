using System;
using LL.Game.Ranks;
using LL.Presentation.Typography;
using LL.UI.Controls;
using TMPro;
using UnityEngine;

namespace LL.UI.Windows.Upgrade.Progress
{
    [DisallowMultipleComponent]
    internal sealed class UpgradeExperienceView : MonoBehaviour
    {
        [SerializeField] private PredictedProgressBar _bar;
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _experienceLabel;
        [SerializeField] private TMP_Text _addedExperienceLabel;
        [SerializeField] private GameObject _maxExperienceMarker;

        internal void Show(RankProgress progress, int currentExperience, int addedExperience)
        {
            _rankLabel.text = progress.RankNumber.ToString();

            if (progress.HasNextRank is false)
            {
                ShowMaxRank(progress, currentExperience);
                return;
            }

            if (addedExperience <= 0)
            {
                ShowCurrentProgress(progress, currentExperience);
                return;
            }

            ShowPreviewProgress(progress, currentExperience, addedExperience);
        }

        private void ShowMaxRank(RankProgress progress, int currentExperience)
        {
            _experienceLabel.text = TextFormatter.Number(currentExperience);
            _addedExperienceLabel.text = string.Empty;
            _maxExperienceMarker.SetActive(true);
            SetBarProgress(progress, currentExperience);
        }

        private void ShowCurrentProgress(RankProgress progress, int currentExperience)
        {
            _experienceLabel.text = TextFormatter.Progress(currentExperience, progress.ExperienceRequiredForRankUp);
            _addedExperienceLabel.text = string.Empty;
            _maxExperienceMarker.SetActive(currentExperience >= progress.ExperienceRequiredForRankUp);
            SetBarProgress(progress, currentExperience);
        }

        private void ShowPreviewProgress(RankProgress progress, int currentExperience, int addedExperience)
        {
            var previewExperience = Math.Min(currentExperience + addedExperience, progress.ExperienceRequiredForRankUp);

            _experienceLabel.text = TextFormatter.Progress(currentExperience, progress.ExperienceRequiredForRankUp);
            _addedExperienceLabel.text = $"+ {TextFormatter.Number(addedExperience)}";
            _maxExperienceMarker.SetActive(previewExperience >= progress.ExperienceRequiredForRankUp);
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