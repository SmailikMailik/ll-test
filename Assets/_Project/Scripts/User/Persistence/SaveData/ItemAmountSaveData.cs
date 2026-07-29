using Newtonsoft.Json;

namespace LL.User.Persistence.SaveData
{
    [JsonObject(MemberSerialization.OptIn, ItemRequired = Required.AllowNull)]
    internal sealed class ItemAmountSaveData
    {
        [JsonProperty] private string _id;
        [JsonProperty] private int _amount;

        internal string Id => _id;
        internal int Amount => _amount;

        [JsonConstructor]
        private ItemAmountSaveData() { }

        internal ItemAmountSaveData(string id, int amount)
        {
            _id = id;
            _amount = amount;
        }
    }
}