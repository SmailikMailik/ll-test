using System;
using System.Collections.Generic;
using System.Linq;
using LL.Rewards;
using LL.UI.Controls.Buttons;
using LL.UI.Rewards;
using R3;
using UnityEngine;

namespace LL.UI.Windows.Views
{
    internal sealed class RewardWindow : Window<RewardWindowParameters>
    {
        [SerializeField] private RewardLayout _rewardLayout;
        [SerializeField] private InteractiveButton _continueButton;

        private void Start()
        {
            _continueButton.Clicked.Subscribe(_ => TryClose()).AddTo(this);
        }

        protected override void OnShow()
        {
            _rewardLayout.SetRewards(Parameters.Rewards);
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