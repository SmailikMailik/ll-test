using System;
using LL.Game.Payments;

namespace LL.Presentation.RankUp
{
    internal interface IRankUpConfirmation
    {
        void Confirm(Payment payment, Action onConfirmed, Action onRejected);
    }
}