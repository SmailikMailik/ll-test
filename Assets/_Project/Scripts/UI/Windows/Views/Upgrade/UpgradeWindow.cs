using LL.Extensions;
using LL.User.Core.Data;
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

        private UserData _userData;
        private IUserProgress _userProgress;
        private IRankProgression _rankProgression;

        [Inject]
        private void Construct(
            UserData userData,
            IUserProgress userProgress,
            IRankProgression rankProgression)
        {
            _userData = userData;
            _userProgress = userProgress;
            _rankProgression = rankProgression;
        }

        protected override void OnShow()
        {
            _rankLabel.text = _rankProgression.GetRank(_userData.TotalExperience).ToString();
            _expLabel.text = $"{_userData.TotalExperience.ToNumber()}/{15000.ToNumber()}";
            _barImage.fillAmount = _userData.TotalExperience / 15000f;
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