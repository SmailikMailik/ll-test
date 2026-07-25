using LL.Game.Currencies;

namespace LL.Helpers
{
    internal static class TextAtlasHelper
    {
        private const string Mask = "<sprite name=\"{0}\">";

        private static readonly string _soft = string.Format(Mask, "T_Soft");
        private static readonly string _hard = string.Format(Mask, "T_Hard");
        private static readonly string _masterPoint = string.Format(Mask, "T_MasterPoint");

        internal static string GetCurrencyIcon(CurrencyId id)
        {
            if (id == CurrencyIds.Soft)
                return _soft;

            if (id == CurrencyIds.Hard)
                return _hard;

            return id == CurrencyIds.MasterPoint
                ? _masterPoint
                : string.Empty;
        }
    }
}