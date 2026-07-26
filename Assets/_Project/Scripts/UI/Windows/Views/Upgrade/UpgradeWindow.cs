using LL.Extensions;
using LL.Game.Ranks;
using LL.User.Core.Progress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Windows.Views.Upgrade
{
    internal sealed class UpgradeWindow : Window<UpgradeWindowParameters>
    {
        [SerializeField] private Image _barImage;
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private TMP_Text _expLabel;

        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;

        [Inject]
        private void Construct(
            IUserProgress userProgress,
            IRankProgression rankProgression)
        {
            _userProgress = userProgress;
            _rankProgression = rankProgression;
        }

        protected override void OnShow()
        {
            var progress = _rankProgression.GetProgress(_userProgress.CurrentTotalExperience);

            _rankLabel.text = progress.Rank.ToString();
            _expLabel.text = progress.HasNextRank
                ? $"{progress.TotalExperience.ToNumber()}/{progress.NextRankExperience.ToNumber()}"
                : progress.TotalExperience.ToNumber();
            _barImage.fillAmount = progress.NormalizedExperience;
        }
    }

    internal sealed class UpgradeWindowParameters : IWindowParameters
    {
        internal int Id { get; }

        internal UpgradeWindowParameters(int id)
        {
            Id = id;
        }
    }
}