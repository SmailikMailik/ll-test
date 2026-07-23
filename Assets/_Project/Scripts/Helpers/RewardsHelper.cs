using UnityEngine;

namespace LL.Helpers
{
    internal enum ItemType : byte
    {
        None = 0,
        Currency = 1,
        Car = 2,
        NoAds = 3
    }

    internal interface IReward
    {
        ItemType ItemType { get; }
        int ItemId { get; }
        int ItemsCount { get; }
    }

    internal sealed class CommonReward : IReward
    {
        public ItemType ItemType { get; }
        public int ItemId { get; }
        public int ItemsCount { get; }

        internal CommonReward(ItemType itemType, int itemId, int itemsCount)
        {
            ItemType = itemType;
            ItemId = itemId;
            ItemsCount = itemsCount;
        }

        internal CommonReward(ItemType itemType, CurrencyType currencyType, int itemsCount)
            : this(itemType, (int)currencyType, itemsCount) { }
    }

    internal static class RewardsHelper
    {
        internal static void AddRewards(params IReward[] rewards)
        {
            if (rewards is null)
                return;

            foreach (var reward in rewards)
            {
                switch (reward.ItemType)
                {
                    case ItemType.Currency:
                        CurrenciesHelper.TryAdd((CurrencyType)reward.ItemId, reward.ItemsCount);
                        break;

                    default:
                        Debug.LogError($"[RewardsHelper::AddReward] Item type '{reward.ItemType}' not defined!");
                        break;
                }
            }
        }

        internal static Sprite GetRewardIcon(IReward reward) => reward.ItemType switch
        {
            //ItemType.Currency => SpriteReferences.Instance.Currencies[(CurrencyType)reward.ItemId],
            _ => null
        };

        internal static string GetRewardText(IReward reward) => reward.ItemType switch
        {
            ItemType.Currency => $"{reward.ItemsCount} {TextAtlasHelper.GetCurrencyIcon((CurrencyType)reward.ItemId)}",
            _ => "Not defined"
        };
    }
}