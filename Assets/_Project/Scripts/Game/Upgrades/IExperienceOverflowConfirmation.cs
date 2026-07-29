using System;

namespace LL.Game.Upgrades
{
    internal interface IExperienceOverflowConfirmation
    {
        void Confirm(int lostExperience, Action onConfirmed, Action onRejected);
    }
}