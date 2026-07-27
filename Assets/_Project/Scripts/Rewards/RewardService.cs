using LL.User.Core.Cards;
using LL.User.Core.Wallet;
using VContainer;

namespace LL.Rewards
{
    internal interface IRewardService
    {
        bool TryApply(IReward reward);
    }

    internal sealed class RewardService : IRewardService
    {
        private readonly IUserWallet _wallet;
        private readonly IUserCards _cards;

        [Inject]
        internal RewardService(
            IUserWallet wallet,
            IUserCards cards)
        {
            _wallet = wallet;
            _cards = cards;
        }

        public bool TryApply(IReward reward) => reward switch
        {
            CurrencyReward currency => _wallet.TryAdd(currency.CurrencyId, currency.Amount),
            CardReward card => _cards.TryAdd(card.CardId, card.Amount),
            _ => false
        };
    }
}