namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class RewardItemDocumentEntry
    {
        public string Id { get; }
        public int Amount { get; }

        public RewardItemDocumentEntry(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}