using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.UI.Windows.Promotion;
using LL.UI.Windows.Reward;
using LL.UI.Windows.Upgrade;
using LL.User.State.Progress;
using VContainer;

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

        internal void CompletePromotion(int rank, IReadOnlyList<ItemAmount> rewardItems)
        {
            ReplaceCurrent();
            _windowController.Show(new RewardWindowParameters(rank, rewardItems));
        }
    }
}