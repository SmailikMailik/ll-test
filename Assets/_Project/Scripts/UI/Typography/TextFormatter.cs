using System;
using System.Collections.Generic;
using System.Globalization;
using LL.Extensions;
using LL.Game.Currencies;

namespace LL.UI.Typography
{
    internal static class TextFormatter
    {
        private static readonly Dictionary<CurrencyId, (TextSprite Sprite, TextStyle Style)> _currencyFormats = new()
        {
            [CurrencyIds.Soft] = (TextSprite.Cash, TextStyle.White),
            [CurrencyIds.Hard] = (TextSprite.Gold, TextStyle.Gold),
            [CurrencyIds.MasterPoint] = (TextSprite.MasterPoints, TextStyle.White)
        };

        private static readonly NumberFormatInfo _numberFormat = new()
        {
            NumberGroupSeparator = TextSymbols.GetValue(TextSymbol.NonBreakingSpace),
            NumberDecimalDigits = 0
        };

        internal static string Number(int value)
        {
            return value.ToString("N0", _numberFormat);
        }

        internal static string Amount(int value)
        {
            return $"x{Number(value)}";
        }

        internal static string Progress(int current, int target)
        {
            return Number(current) + TextTags.Style($"/{Number(target)}", TextStyle.Muted);
        }

        internal static string Duration(TimeSpan duration, string hoursUnit, string minutesUnit)
        {
            if (duration < TimeSpan.Zero)
                duration = TimeSpan.Zero;

            var hours = duration.GetWholeHours();
            var minutes = duration.Minutes.ToString("00", CultureInfo.InvariantCulture);
            var separator = TextSymbols.GetValue(TextSymbol.NonBreakingSpace);
            var time = $"{hours}{hoursUnit}{separator}{minutes}{minutesUnit}";
            return $"{TextTags.Sprite(TextSprite.Time)}{separator}{time}";
        }

        internal static string CurrencyAmount(CurrencyId id, int amount)
        {
            var formattedAmount = Number(amount);

            if (_currencyFormats.TryGetValue(id, out var format) is false)
                return formattedAmount;

            var sprite = TextTags.Sprite(format.Sprite);
            var separator = TextSymbols.GetValue(TextSymbol.NonBreakingSpace);
            return TextTags.Style($"{sprite}{separator}{formattedAmount}", format.Style);
        }
    }
}