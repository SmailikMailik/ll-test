using System;
using System.Globalization;

namespace LL.Presentation.Typography
{
    internal static class TextFormatter
    {
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
            return value > 1 ? $"x{Number(value)}" : string.Empty;
        }

        internal static string Progress(int current, int target)
        {
            return Number(current) + TextTags.Style($"/{Number(target)}", TextStyle.Muted);
        }

        internal static string Duration(
            TimeSpan duration,
            string hoursUnit,
            string minutesUnit,
            string secondsUnit)
        {
            if (duration < TimeSpan.Zero)
                duration = TimeSpan.Zero;

            var totalHours = (int)duration.TotalHours;
            var minutes = duration.Minutes.ToString("D2", CultureInfo.InvariantCulture);
            var seconds = duration.Seconds.ToString("D2", CultureInfo.InvariantCulture);

            var separator = TextSymbols.GetValue(TextSymbol.NonBreakingSpace);
            var time = $"{totalHours}{hoursUnit}{separator}{minutes}{minutesUnit}{separator}{seconds}{secondsUnit}";
            return $"{TextTags.Sprite(TextSprite.Time)}{separator}{time}";
        }
    }
}