using System;
using LL.User.Core.Cards;
using LL.User.Core.Items;
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
        private readonly IUserItems _items;
        private readonly IUserCards _cards;

        [Inject]
        internal RewardService(
            IUserItems items,
            IUserCards cards)
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