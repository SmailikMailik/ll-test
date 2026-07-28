using System;

namespace LL.Extensions
{
    internal static class TimeSpanExtensions
    {
        private const int HoursPerDay = 24;

        internal static int GetWholeHours(this TimeSpan value)
        {
            return value.Days * HoursPerDay + value.Hours;
        }
    }
}