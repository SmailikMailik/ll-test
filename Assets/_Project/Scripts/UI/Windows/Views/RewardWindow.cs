using LL.Helpers;
using UnityEngine;

namespace LL.UI.Windows.Views
{
    internal sealed class RewardWindow : Window<RewardWindowParameters>
    {
        [SerializeField] private RewardsContainer _container;
        [SerializeField] private CommonButton _continueButton;

        private void OnEnable()
        {
            _continueButton.Clicked += Back;
        }

        private void OnDisable()
        {
            _continueButton.Clicked -= Back;
        }

        protected override void OnShow()
        {
            _container.ShowRewardsDelayed(Parameters.Rewards);
        }
    }

    internal sealed class RewardWindowParameters : IWindowParameters
    {
        internal IReward[] Rewards { get; }

        internal RewardWindowParameters(params IReward[] rewards)
        {
            Rewards = rewards;
        }
    }
}