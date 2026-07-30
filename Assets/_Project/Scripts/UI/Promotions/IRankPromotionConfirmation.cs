using System;
using LL.Game.Payments;

namespace LL.UI.Promotions
{
    internal interface IRankPromotionConfirmation
    {
        void Confirm(Payment payment, Action onConfirmed, Action onRejected);
    }
}