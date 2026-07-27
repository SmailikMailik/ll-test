using System.Collections.Generic;
using LL.Game.Purchases;
using UnityEngine;

namespace LL.Presentation.Purchasing
{
    internal static class PurchaseLocalizationKeys
    {
        internal const string ConfirmationTitle = "Common/titles.confirm_purchase";
        internal const string PurchaseAction = "Common/actions.purchase";
        internal const string CancelAction = "Common/actions.cancel";

        private const string DefaultConfirmation = "Common/messages.confirm_purchase";

        private static readonly Dictionary<PurchaseId, string> _confirmationByPurchaseId = new()
        {
            [PurchaseIds.RankPromotion] = "Upgrade/messages.confirm_rank_promotion",
            [PurchaseIds.InstantRankPromotion] = "Upgrade/messages.confirm_instant_rank_promotion",
        };

        internal static string GetConfirmation(PurchaseId id)
        {
            if (_confirmationByPurchaseId.TryGetValue(id, out var localizationKey))
                return localizationKey;

            Debug.LogError($"Purchase confirmation is not configured for ID: {id}");
            return DefaultConfirmation;
        }
    }
}