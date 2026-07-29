namespace LL.User.Persistence.SaveData
{
    internal sealed class ItemAmountSaveData
    {
        public string Id { get; }
        public int Amount { get; }

        public ItemAmountSaveData(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }
    }
}