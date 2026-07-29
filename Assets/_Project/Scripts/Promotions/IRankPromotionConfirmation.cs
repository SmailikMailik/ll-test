using System;
using LL.Payments;

namespace LL.Promotions
{
    internal interface IRankPromotionConfirmation
    {
        void Confirm(Payment payment, Action onConfirmed, Action onRejected);
    }
}