using LL.Game.Cards;

namespace LL.Rewards.Models
{
    internal sealed class CardReward : IReward
    {
        internal CardId CardId { get; }
        public int Amount { get; }

        internal CardReward(CardId cardId, int amount)
        {
            CardId = cardId;
            Amount = amount;
        }
    }
}