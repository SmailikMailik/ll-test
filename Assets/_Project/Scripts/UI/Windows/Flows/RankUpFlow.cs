using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.RankUp;
using LL.Game.RankUp.Services;
using LL.Presentation.RankUp;
using VContainer;

namespace LL.UI.Windows.Flows
{
    internal sealed class RankUpFlow
    {
        private readonly IRankUpService _rankUpService;
        private readonly IRankUpConfirmation _confirmation;

        private bool _isPending;

        [Inject]
        internal RankUpFlow(
            IRankUpService rankUpService,
            IRankUpConfirmation confirmation)
        {
            _rankUpService = rankUpService ?? throw new ArgumentNullException(nameof(rankUpService));
            _confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));
        }

        internal bool TryGetDefinition(HeroId heroId, out RankUpDefinition definition)
        {
            return _rankUpService.TryGetDefinition(heroId, out definition);
        }

        internal void RequestRankUp(
            HeroId heroId,
            RankUpOptionId optionId,
            Action<IReadOnlyList<ItemAmount>> onSucceeded,
            Action onFailed)
        {
            if (_isPending ||
                _rankUpService.TryGetDefinition(heroId, out var definition) is false ||
                definition.TryGetOption(optionId, out var option) is false ||
                _rankUpService.CanCompleteOption(heroId, optionId) is false)
            {
                onFailed?.Invoke();
                return;
            }

            _isPending = true;
            _confirmation.Confirm(
                heroId,
                option,
                () => OnRankUpConfirmed(heroId, optionId, onSucceeded, onFailed),
                () => OnRankUpRejected(onFailed));
        }

        private void OnRankUpConfirmed(
            HeroId heroId,
            RankUpOptionId optionId,
            Action<IReadOnlyList<ItemAmount>> onSucceeded,
            Action onFailed)
        {
            if (_isPending is false)
                return;

            if (_rankUpService.TryCompleteOption(heroId, optionId, out var rewardItems))
                OnRankUpSucceeded(rewardItems, onSucceeded);
            else
                OnRankUpFailed(onFailed);
        }

        private void OnRankUpRejected(Action onFailed)
        {
            OnRankUpFailed(onFailed);
        }

        private void OnRankUpSucceeded(
            IReadOnlyList<ItemAmount> items,
            Action<IReadOnlyList<ItemAmount>> onSucceeded)
        {
            if (_isPending is false)
                return;

            _isPending = false;
            onSucceeded?.Invoke(items);
        }

        private void OnRankUpFailed(Action onFailed)
        {
            if (_isPending is false)
                return;

            _isPending = false;
            onFailed?.Invoke();
        }
    }
}