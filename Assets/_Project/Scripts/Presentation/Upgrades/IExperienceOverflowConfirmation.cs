using System;

namespace LL.Presentation.Upgrades
{
    internal interface IExperienceOverflowConfirmation
    {
        void Confirm(int lostExperience, Action onConfirmed, Action onRejected);
    }
}