using System;

namespace LL.Purchasing
{
    internal interface IPurchaseService
    {
        void Purchase(IPurchase purchase, Action onSucceeded, Action onFailed);
    }
}