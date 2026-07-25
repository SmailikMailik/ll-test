using LL.Helpers;
using UnityEngine;

namespace LL.Rewards
{
    internal static class RewardFormatter
    {
        internal static Sprite GetIcon(IReward reward) => null;

        internal static string GetText(IReward reward) => reward switch
        {
            CurrencyReward currency => $"{currency.Amount} {TextAtlasHelper.GetCurrencyIcon(currency.CurrencyId)}",
            ExperienceReward experience => $"{experience.Amount} XP",
            _ => string.Empty
        };
    }
}