using System;
using LL.Game.Payments;

namespace LL.Presentation.Promotions
{
    internal interface IRankPromotionConfirmation
    {
        void Confirm(Payment payment, Action onConfirmed, Action onRejected);
    }
}