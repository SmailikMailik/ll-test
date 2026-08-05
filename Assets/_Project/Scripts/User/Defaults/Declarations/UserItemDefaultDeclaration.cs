namespace LL.User.Defaults.Declarations
{
    internal sealed class UserItemDefaultDeclaration
    {
        internal string Id { get; }
        internal int Amount { get; }

        internal UserItemDefaultDeclaration(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}