using System;
using LL.Game.Purchases;

namespace LL.Purchasing
{
    internal interface IPurchaseService
    {
        bool TryGetPurchase(PurchaseId id, out IPurchase purchase);
        void Purchase(PurchaseId id, Action onSucceeded, Action onFailed);
        void Purchase(IPurchase purchase, Action onSucceeded, Action onFailed);
    }
}