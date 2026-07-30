namespace LL.User.Persistence.Documents
{
    internal sealed class ItemAmountDocumentEntry
    {
        public string Id { get; }
        public int Amount { get; }

        public ItemAmountDocumentEntry(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}