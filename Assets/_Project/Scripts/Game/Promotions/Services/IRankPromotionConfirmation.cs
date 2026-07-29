using System;
using LL.Game.Payments;

namespace LL.Game.Promotions.Services
{
    internal interface IRankPromotionConfirmation
    {
        void Confirm(Payment payment, Action onConfirmed, Action onRejected);
    }
}