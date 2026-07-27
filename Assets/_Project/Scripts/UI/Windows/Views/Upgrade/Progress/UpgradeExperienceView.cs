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

        internal void Show(
            string rank,
            string experience,
            string addedExperience,
            float currentProgress,
            float previewProgress)
        {
            _rankLabel.text = rank;
            _experienceLabel.text = experience;
            _addedExperienceLabel.text = addedExperience;
            _bar.SetProgress(currentProgress, previewProgress);
        }
    }
}