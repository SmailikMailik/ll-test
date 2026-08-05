using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.UI.Windows.RankUp;
using LL.UI.Windows.Reward;
using LL.UI.Windows.Upgrade;
using LL.User.Snapshots;
using LL.User.State.Heroes;
using VContainer;

namespace LL.UI.Windows.Flows
{
    internal sealed class UpgradeFlow
    {
        private readonly WindowController _windowController;
        private readonly IUserHeroProgress _userProgress;
        private readonly UserHeroSelectionSnapshot _heroSelection;

        [Inject]
        internal UpgradeFlow(
            WindowController windowController,
            IUserHeroProgress userProgress,
            UserHeroSelectionSnapshot heroSelection)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _heroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
        }

        internal void Open()
        {
            if (_userProgress.CanRankUp(_heroSelection.HeroId))
                _windowController.Show(new RankUpWindowParameters(_heroSelection.HeroId));
            else
                _windowController.Show(new UpgradeWindowParameters(_heroSelection.HeroId));
        }

        internal void ReplaceCurrent()
        {
            if (_userProgress.CanRankUp(_heroSelection.HeroId))
                _windowController.Replace(new RankUpWindowParameters(_heroSelection.HeroId));
            else
                _windowController.Replace(new UpgradeWindowParameters(_heroSelection.HeroId));
        }

        internal void CompleteRankUp(int rankNumber, IReadOnlyList<ItemAmount> rewardItems)
        {
            ReplaceCurrent();
            _windowController.Show(new RewardWindowParameters(rankNumber, rewardItems));
        }
    }
}