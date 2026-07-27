using LL.User.Core.Cards;
using LL.User.Core.Items;
using LL.User.Core.Wallet;
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
        private readonly IUserWallet _wallet;
        private readonly IUserCards _cards;
        private readonly IUserItems _items;

        [Inject]
        internal RewardService(
            IUserWallet wallet,
            IUserCards cards,
            IUserItems items)
        {
            _wallet = wallet;
            _cards = cards;
            _items = items;
        }

        public bool CanApply(IReward reward) => reward switch
        {
            CurrencyReward currency => _wallet.CanAdd(currency.CurrencyId, currency.Amount),
            ItemReward item => _items.CanAdd(item.ItemId, item.Amount),
            CardReward card => _cards.CanAdd(card.CardId, card.Amount),
            _ => false
        };

        public bool TryApply(IReward reward) => reward switch
        {
            CurrencyReward currency => _wallet.TryAdd(currency.CurrencyId, currency.Amount),
            ItemReward item => _items.TryAdd(item.ItemId, item.Amount),
            CardReward card => _cards.TryAdd(card.CardId, card.Amount),
            _ => false
        };
    }
}