namespace LL.User.Defaults.Declarations
{
    internal sealed class UserItemDefaultsDeclaration
    {
        internal string Id { get; }
        internal int Amount { get; }

        internal UserItemDefaultsDeclaration(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}