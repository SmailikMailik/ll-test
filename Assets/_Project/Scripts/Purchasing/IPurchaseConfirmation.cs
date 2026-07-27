using System;
using LL.Game.Purchases;

namespace LL.Purchasing
{
    internal interface IPurchaseConfirmation
    {
        void Confirm(IPurchase purchase, Action onConfirmed, Action onRejected);
    }
}