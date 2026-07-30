using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Promotions;
using LL.Game.Promotions.Services;
using LL.UI.Promotions;
using VContainer;

namespace LL.UI.Windows.Flows
{
    internal sealed class RankPromotionFlow
    {
        private readonly IRankPromotionService _promotionService;
        private readonly IRankPromotionConfirmation _confirmation;

        private bool _isPending;

        [Inject]
        internal RankPromotionFlow(
            IRankPromotionService promotionService,
            IRankPromotionConfirmation confirmation)
        {
            _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
            _confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));
        }

        internal bool TryGetPromotion(out RankPromotion promotion)
        {
            return _promotionService.TryGetPromotion(out promotion);
        }

        internal void Promote(
            Payment payment,
            Action<IReadOnlyList<ItemAmount>> onSucceeded,
            Action onFailed)
        {
            if (_isPending ||
                _promotionService.CanPromote(payment) is false)
            {
                onFailed?.Invoke();
                return;
            }

            _isPending = true;
            _confirmation.Confirm(
                payment,
                () => OnPromotionConfirmed(payment, onSucceeded, onFailed),
                () => OnPromotionRejected(onFailed));
        }

        private void OnPromotionConfirmed(
            Payment payment,
            Action<IReadOnlyList<ItemAmount>> onSucceeded,
            Action onFailed)
        {
            if (_isPending is false)
                return;

            if (_promotionService.TryPromote(payment, out var rewardItems))
                OnPromotionSucceeded(rewardItems, onSucceeded);
            else
                OnPromotionFailed(onFailed);
        }

        private void OnPromotionRejected(Action onFailed)
        {
            OnPromotionFailed(onFailed);
        }

        private void OnPromotionSucceeded(
            IReadOnlyList<ItemAmount> items,
            Action<IReadOnlyList<ItemAmount>> onSucceeded)
        {
            if (_isPending is false)
                return;

            _isPending = false;
            onSucceeded?.Invoke(items);
        }

        private void OnPromotionFailed(Action onFailed)
        {
            if (_isPending is false)
                return;

            _isPending = false;
            onFailed?.Invoke();
        }
    }
}