using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Payments;
using LL.Game.Payments.Services;
using LL.Game.Rewards.Services;
using LL.User.State.Progress;
using LL.User.State.RankUp;
using UnityEngine;
using VContainer;

namespace LL.Game.RankUp.Services
{
    internal sealed class RankUpService : IRankUpService
    {
        private readonly RankUpCatalog _catalog;
        private readonly IUserProgress _userProgress;
        private readonly IUserRankUpQuest _rankUpQuest;
        private readonly IPaymentService _paymentService;
        private readonly IRewardGrantService _rewardGrantService;

        [Inject]
        internal RankUpService(
            RankUpCatalog catalog,
            IUserProgress userProgress,
            IUserRankUpQuest rankUpQuest,
            IPaymentService paymentService,
            IRewardGrantService rewardGrantService)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _userProgress = userProgress ?? throw new ArgumentNullException(nameof(userProgress));
            _rankUpQuest = rankUpQuest ?? throw new ArgumentNullException(nameof(rankUpQuest));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _rewardGrantService = rewardGrantService ?? throw new ArgumentNullException(nameof(rewardGrantService));
        }

        public bool TryGetDefinition(out RankUpDefinition definition)
        {
            return _catalog.TryGetDefinition(_userProgress.RankId, out definition);
        }

        public bool CanRankUp(Payment payment)
        {
            return TryGetDefinition(out var definition) && CanRankUp(definition, payment);
        }

        public bool TryRankUp(
            Payment payment,
            out IReadOnlyList<ItemAmount> rewardItems)
        {
            rewardItems = Array.Empty<ItemAmount>();

            if (TryGetDefinition(out var definition) is false ||
                CanRankUp(definition, payment) is false)
                return false;

            if (_rewardGrantService.CanGrant(definition.RewardId) is false ||
                _paymentService.TryPay(payment) is false)
                return false;

            if (_userProgress.TryRankUp() is false)
            {
                RefundPayment(payment);
                return false;
            }

            rewardItems = GrantReward(definition);
            _rankUpQuest.ClearQuest();
            return true;
        }

        private bool CanRankUp(RankUpDefinition definition, Payment payment)
        {
            if (_userProgress.RankId.Equals(definition.RankId) is false ||
                _userProgress.CanRankUp is false)
            {
                return false;
            }

            if (Matches(payment, definition.Quest.Payment))
            {
                return _rankUpQuest.IsCompleted &&
                       _rankUpQuest.QuestId.Equals(definition.Quest.QuestId);
            }

            return Matches(payment, definition.InstantPayment);
        }

        private static bool Matches(Payment payment, Payment expected)
        {
            return payment.ItemId.Equals(expected.ItemId) && payment.Amount == expected.Amount;
        }

        private IReadOnlyList<ItemAmount> GrantReward(RankUpDefinition definition)
        {
            if (_rewardGrantService.TryGrant(definition.RewardId, out var items))
                return items;

            Debug.LogError($"Failed to grant rank-up reward: {definition.RewardId}");
            return Array.Empty<ItemAmount>();
        }

        private void RefundPayment(Payment payment)
        {
            if (_paymentService.TryRefund(payment) is false)
                Debug.LogError($"Failed to refund rank-up payment: {payment.Amount} {payment.ItemId}");
        }
    }
}