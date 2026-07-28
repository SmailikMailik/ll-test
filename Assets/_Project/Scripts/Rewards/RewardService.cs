using System;
using LL.Game.Cards;
using LL.Game.Items;
using LL.User.Core.Amounts;
using VContainer;

namespace LL.Rewards
{
    internal interface IRewardService
    {
        bool CanApply(IReward reward);
        bool TryApply(IReward reward);
    }

    internal sealed class RewardService : IRewardService
    {
        private readonly IUserAmounts<ItemId> _items;
        private readonly IUserAmounts<CardId> _cards;

        [Inject]
        internal RewardService(
            IUserAmounts<ItemId> items,
            IUserAmounts<CardId> cards)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _cards = cards ?? throw new ArgumentNullException(nameof(cards));
        }

        public bool CanApply(IReward reward) => reward switch
        {
            ItemReward item => _items.CanAdd(item.ItemId, item.Amount),
            CardReward card => _cards.CanAdd(card.CardId, card.Amount),
            _ => false
        };

        public bool TryApply(IReward reward) => reward switch
        {
            ItemReward item => _items.TryAdd(item.ItemId, item.Amount),
            CardReward card => _cards.TryAdd(card.CardId, card.Amount),
            _ => false
        };
    }
}