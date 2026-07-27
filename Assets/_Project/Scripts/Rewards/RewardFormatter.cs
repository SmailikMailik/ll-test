using LL.UI.Typography;
using UnityEngine;

namespace LL.Rewards
{
    internal static class RewardFormatter
    {
        internal static Sprite GetIcon(IReward reward) => null;

        internal static string GetText(IReward reward) => reward switch
        {
            CurrencyReward currency => TextFormatter.CurrencyAmount(currency.CurrencyId, currency.Amount),
            ExperienceReward experience => $"{experience.Amount} XP",
            _ => string.Empty
        };
    }
}