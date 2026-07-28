using LL.Game.Cards;
using LL.Game.Items;

namespace LL.Rewards
{
    internal interface IReward
    {
        int Amount { get; }
    }

    internal sealed class ItemReward : IReward
    {
        internal ItemId ItemId { get; }
        public int Amount { get; }

        internal ItemReward(ItemId itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }

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