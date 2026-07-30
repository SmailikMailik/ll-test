namespace LL.Game.Data.Declarations
{
    internal sealed class ItemAmountDeclaration
    {
        internal string Id { get; }
        internal int Amount { get; }

        internal ItemAmountDeclaration(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}