using LL.Game.Purchases;

namespace LL.UI.Typography
{
    internal static class PurchaseFormatter
    {
        internal static string GetPriceText(IPurchase purchase) => purchase == null
            ? string.Empty
            : TextFormatter.CurrencyAmount(purchase.CurrencyId, purchase.Price);
    }
}