using System;
using System.Collections.Generic;
using System.Linq;

namespace LL.Infrastructure.Collections
{
    internal static class EnumerableExtensions
    {
        internal static IReadOnlyList<T> ToReadOnlyCopy<T>(this IEnumerable<T> source)
        {
            return Array.AsReadOnly(source?.ToArray() ?? Array.Empty<T>());
        }
    }
}