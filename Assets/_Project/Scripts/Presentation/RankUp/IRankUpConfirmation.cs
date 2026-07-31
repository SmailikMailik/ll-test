using System;
using LL.Game.Heroes;
using LL.Game.Payments;

namespace LL.Presentation.RankUp
{
    internal interface IRankUpConfirmation
    {
        void Confirm(HeroId heroId, Payment payment, Action onConfirmed, Action onRejected);
    }
}