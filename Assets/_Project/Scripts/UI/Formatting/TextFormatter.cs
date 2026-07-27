using System.Collections.Generic;
using System.Globalization;
using LL.Game.Currencies;
using LL.UI.Styling;

namespace LL.UI.Formatting
{
    internal static class TextFormatter
    {
        private static readonly Dictionary<CurrencyId, CurrencyFormat> _currencyFormats = new()
        {
            [CurrencyIds.Soft] = new CurrencyFormat(TextSprites.Cash, TextStyles.White),
            [CurrencyIds.Hard] = new CurrencyFormat(TextSprites.Gold, TextStyles.Gold),
            [CurrencyIds.MasterPoint] = new CurrencyFormat(TextSprites.MasterPoints, TextStyles.White)
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

        internal static string CurrencyAmount(CurrencyId id, int amount)
        {
            var formattedAmount = Number(amount);

            if (_currencyFormats.TryGetValue(id, out var format) is false)
                return formattedAmount;

            var sprite = RichText.Sprite(format.Sprite);
            return RichText.Style($"{sprite} {formattedAmount}", format.Style);
        }

        private readonly struct CurrencyFormat
        {
            internal string Sprite { get; }
            internal string Style { get; }

            internal CurrencyFormat(string sprite, string style)
            {
                Sprite = sprite;
                Style = style;
            }
        }
    }
}