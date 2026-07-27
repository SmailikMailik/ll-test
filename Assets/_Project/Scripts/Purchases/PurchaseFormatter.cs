using LL.UI.Typography;

namespace LL.Purchases
{
    internal static class PurchaseFormatter
    {
        internal static string GetPriceText(IPurchase purchase) => purchase == null
            ? string.Empty
            : TextFormatter.CurrencyAmount(purchase.CurrencyId, purchase.Price);
    }
}