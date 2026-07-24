using LL.Helpers;

namespace LL.Purchases
{
    internal static class PurchaseFormatter
    {
        internal static string GetPriceText(IPurchase purchase) => purchase == null
            ? string.Empty
            : $"{purchase.Price} {TextAtlasHelper.GetCurrencyIcon(purchase.Currency)}";
    }
}