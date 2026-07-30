namespace LL.Game.Data.Persistence.Documents
{
    internal sealed class PaymentDocumentEntry
    {
        public string ItemId { get; }
        public int Amount { get; }

        public PaymentDocumentEntry(string itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }
}