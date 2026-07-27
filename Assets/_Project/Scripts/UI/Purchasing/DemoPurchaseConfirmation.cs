using System;
using LL.Game.Purchases;
using LL.Purchasing;
using LL.UI.Typography;
using LL.UI.Windows;
using LL.UI.Windows.Views;
using VContainer;

namespace LL.UI.Purchasing
{
    internal sealed class DemoPurchaseConfirmation : IPurchaseConfirmation
    {
        private readonly WindowController _windowController;

        [Inject]
        internal DemoPurchaseConfirmation(WindowController windowController)
        {
            _windowController = windowController ?? throw new ArgumentNullException(nameof(windowController));
        }

        public void Confirm(IPurchase purchase, Action onConfirmed, Action onRejected)
        {
            _windowController.Show(new ModalWindowParameters
            (
                "Confirm Purchase",
                PurchaseFormatter.GetPriceText(purchase),
                "Yes",
                onConfirmed,
                negativeText: "No",
                negativeCallback: onRejected,
                closeCallback: onRejected
            ));
        }
    }
}