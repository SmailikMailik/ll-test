using LL.Extensions;
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
            var totalExperience = _userProgress.CurrentTotalExperience;

            _rankLabel.text = _rankProgression.GetRank(totalExperience).ToString();
            _expLabel.text = $"{totalExperience.ToNumber()}/{15000.ToNumber()}";
            _barImage.fillAmount = totalExperience / 15000f;
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