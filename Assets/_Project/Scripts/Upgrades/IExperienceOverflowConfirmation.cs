using System;

namespace LL.Upgrades
{
    internal interface IExperienceOverflowConfirmation
    {
        void Confirm(int lostExperience, Action onConfirmed, Action onRejected);
    }
}