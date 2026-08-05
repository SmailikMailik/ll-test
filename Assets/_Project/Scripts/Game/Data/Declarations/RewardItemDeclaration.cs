namespace LL.Game.Data.Declarations
{
    internal sealed class RewardItemDeclaration
    {
        internal string Id { get; }
        internal int Amount { get; }

        internal RewardItemDeclaration(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}