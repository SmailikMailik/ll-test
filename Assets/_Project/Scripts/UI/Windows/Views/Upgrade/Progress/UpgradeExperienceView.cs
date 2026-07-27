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

        internal void Show(RankProgress progress, int currentExperience, int addedExperience)
        {
            _rankLabel.text = progress.Rank.ToString();
            _experienceLabel.text = progress.HasNextRank
                ? TextFormatter.Progress(progress.TotalExperience, progress.NextRankExperience)
                : TextFormatter.Number(progress.TotalExperience);
            _addedExperienceLabel.text = addedExperience > 0
                ? $"+ {TextFormatter.Number(addedExperience)}"
                : string.Empty;
            _bar.SetProgress(
                progress.GetNormalizedExperience(currentExperience),
                progress.NormalizedExperience);
        }
    }
}