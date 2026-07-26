using LL.UI.Formatting;

namespace LL.Purchases
{
    internal static class PurchaseFormatter
    {
        internal static string GetPriceText(IPurchase purchase) => purchase == null
            ? string.Empty
            : $"{purchase.Price} {TextFormatter.CurrencySprite(purchase.CurrencyId)}";
    }
}