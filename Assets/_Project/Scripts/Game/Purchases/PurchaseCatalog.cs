using System;
using System.Collections.Generic;
using System.Linq;
using LL.Identifiers;

namespace LL.Game.Purchases
{
    internal sealed class PurchaseCatalog
    {
        private readonly IReadOnlyDictionary<PurchaseId, IPurchase> _purchases;

        internal PurchaseCatalog(IEnumerable<IPurchase> purchases)
        {
            var copy = purchases?.ToArray() ?? Array.Empty<IPurchase>();
            IdentifierCatalogValidator.EnsureValidIds(
                copy,
                purchase => purchase.Id,
                nameof(PurchaseCatalog),
                nameof(purchases));

            _purchases = copy.ToDictionary(purchase => purchase.Id);
        }

        internal bool TryGetPurchase(PurchaseId id, out IPurchase purchase) => _purchases.TryGetValue(id, out purchase);
    }
}