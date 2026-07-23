using LL.Helpers;
using LL.UI.Windows.Core;
using UnityEngine;

namespace LL.UI.Windows.View
{
    internal sealed class RewardWindow : WindowParameterized<RewardParameters>
    {
        [SerializeField] private RewardsContainer _container;
        [SerializeField] private CommonButton _continueButton;

        internal override void Init()
        {
            base.Init();

            _continueButton.Clicked += Back;
        }

        internal override void Show()
        {
            base.Show();

            _container.ShowRewardsDelayed(Parameters.Rewards);
        }
    }

    internal sealed class RewardParameters : IWindowParameters
    {
        internal IReward[] Rewards { get; }

        internal RewardParameters(params IReward[] rewards)
        {
            Rewards = rewards;
        }
    }
}