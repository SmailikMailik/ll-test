using System.Collections.Generic;
using System.Globalization;
using LL.Game.Currencies;
using LL.UI.Styling;

namespace LL.UI.Formatting
{
    internal static class TextFormatter
    {
        private static readonly Dictionary<CurrencyId, string> _currencySprites = new()
        {
            [CurrencyIds.Soft] = TextSprites.Cash,
            [CurrencyIds.Hard] = TextSprites.Gold,
            [CurrencyIds.MasterPoint] = TextSprites.MasterPoints
        };

        private static readonly NumberFormatInfo _numberFormat = new()
        {
            NumberGroupSeparator = "\u00A0", // Неразрывный пробел
            NumberDecimalDigits = 0
        };

        internal static string Number(int value)
        {
            return value.ToString("N0", _numberFormat);
        }

        internal static string Progress(int current, int target)
        {
            return Number(current) + RichText.Style($"/{Number(target)}", TextStyles.Muted);
        }

        internal static string CurrencySprite(CurrencyId id)
        {
            return _currencySprites.TryGetValue(id, out var sprite)
                ? RichText.Sprite(sprite)
                : string.Empty;
        }
    }
}