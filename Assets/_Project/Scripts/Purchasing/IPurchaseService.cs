using System;
using LL.Game.Purchases;

namespace LL.Purchasing
{
    internal interface IPurchaseService
    {
        void Purchase(IPurchase purchase, Action onSucceeded, Action onFailed);
    }
}