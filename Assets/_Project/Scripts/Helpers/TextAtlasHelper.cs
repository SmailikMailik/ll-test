namespace LL.Helpers
{
    internal static class TextAtlasHelper
    {
        private const string Mask = "<sprite name=\"{0}\">";

        private static readonly string _soft = string.Format(Mask, "T_Soft");
        private static readonly string _hard = string.Format(Mask, "T_Hard");

        internal static string GetCurrencyIcon(CurrencyType type) => type switch
        {
            CurrencyType.Soft => _soft,
            CurrencyType.Hard => _hard,
            _ => string.Empty
        };
    }
}