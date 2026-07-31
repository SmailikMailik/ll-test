namespace LL.User.Persistence.Documents
{
    internal sealed class UserItemDocumentEntry
    {
        public string Id { get; }
        public int Amount { get; }

        public UserItemDocumentEntry(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}