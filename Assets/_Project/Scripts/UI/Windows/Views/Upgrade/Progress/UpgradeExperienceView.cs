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
            var previewExperience = progress.HasNextRank
                ? (int)Math.Min((long)currentExperience + addedExperience, progress.RequiredExperience)
                : currentExperience;

            _rankLabel.text = progress.Rank.ToString();
            _experienceLabel.text = progress.HasNextRank
                ? TextFormatter.Progress(previewExperience, progress.RequiredExperience)
                : TextFormatter.Number(previewExperience);
            _addedExperienceLabel.text = addedExperience > 0
                ? $"+ {TextFormatter.Number(addedExperience)}"
                : string.Empty;
            _maximumExperienceMarker.SetActive(
                progress.HasNextRank is false ||
                previewExperience >= progress.RequiredExperience);
            _bar.SetProgress(
                progress.GetNormalizedExperience(currentExperience),
                progress.GetNormalizedExperience(previewExperience));
        }
    }
}