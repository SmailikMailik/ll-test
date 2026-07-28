using System;

namespace LL.Presentation.Orders
{
    internal interface IOrderCompletionConfirmation
    {
        void Confirm(Action onConfirmed);
    }
}