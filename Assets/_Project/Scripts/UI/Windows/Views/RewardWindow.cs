using System;
using System.Collections.Generic;
using System.Linq;
using LL.Rewards;
using R3;
using UnityEngine;

namespace LL.UI.Windows.Views
{
    internal sealed class RewardWindow : Window<RewardWindowParameters>
    {
        [SerializeField] private RewardsContainer _container;
        [SerializeField] private CommonButton _continueButton;

        private void Start()
        {
            _continueButton.Clicked.Subscribe(_ => TryClose()).AddTo(this);
        }

        protected override void OnShow()
        {
            _container.ShowRewardsDelayed(Parameters.Rewards);
        }
    }

    internal sealed class RewardWindowParameters : IWindowParameters
    {
        internal IReadOnlyList<IReward> Rewards { get; }

        internal RewardWindowParameters(params IReward[] rewards)
        {
            Rewards = Array.AsReadOnly(rewards?.ToArray() ?? Array.Empty<IReward>());
        }
    }
}