using System;
using System.Collections.Generic;
using System.Linq;
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
            _continueButton.Clicked += OnContinueClicked;
        }

        private void OnDisable()
        {
            _continueButton.Clicked -= OnContinueClicked;
        }

        protected override void OnShow()
        {
            _container.ShowRewardsDelayed(Parameters.Rewards);
        }

        private void OnContinueClicked() => TryClose();
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