using System;
using System.Collections.Generic;
using LL.Rewards.Models;
using LL.UI.Windows.Views.Promotion;
using LL.UI.Windows.Views.Reward;
using LL.UI.Windows.Views.Upgrade;
using LL.User.Core.Progress;
using VContainer;
using VContainer.Unity;

namespace LL.UI.Windows.Flows
{
    internal sealed class UpgradeFlow
    {
        private readonly WindowController _windowController;
        private readonly IUserProgress _userProgress;

        [Inject]
        internal UpgradeFlow(
            WindowController windowController,
            IUserProgress userProgress)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
        }

        internal void Open()
        {
            if (_userProgress.CanPromoteRank)
                _windowController.Show(new PromotionWindowParameters());
            else
                _windowController.Show(new UpgradeWindowParameters());
        }

        internal void ReplaceCurrent()
        {
            if (_userProgress.CanPromoteRank)
                _windowController.Replace(new PromotionWindowParameters());
            else
                _windowController.Replace(new UpgradeWindowParameters());
        }

        internal void CompletePromotion(int rank, IReadOnlyList<IReward> rewards)
        {
            ReplaceCurrent();
            _windowController.Show(new RewardWindowParameters(rank, rewards));
        }
    }

    internal sealed class UpgradeFlowStartup : IStartable
    {
        private readonly UpgradeFlow _flow;

        [Inject]
        internal UpgradeFlowStartup(UpgradeFlow flow)
        {
            _flow = flow ?? throw new ArgumentNullException(nameof(flow));
        }

        public void Start()
        {
            _flow.Open();
        }
    }
}