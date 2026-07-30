namespace LL.Game.Data.Declarations
{
    internal sealed class PaymentDeclaration
    {
        internal string ItemId { get; }
        internal int Amount { get; }

        internal PaymentDeclaration(string itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }
}