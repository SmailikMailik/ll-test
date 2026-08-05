using System;
using LL.Game.Heroes;
using LL.Game.RankUp;

namespace LL.Presentation.RankUp
{
    internal interface IRankUpConfirmation
    {
        void Confirm(
            HeroId heroId,
            RankUpOptionDefinition option,
            Action onConfirmed,
            Action onRejected);
    }
}