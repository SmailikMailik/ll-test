using System;
using System.Collections.Generic;

namespace LL.Game.Identifiers
{
    internal static class IdentifierCatalogValidator
    {
        internal static bool HasValidIds<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> idSelector)
            where TId : struct, IIdentifier
        {
            return TryValidateIds(entries, idSelector, out _);
        }

        internal static void EnsureValidIds<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> idSelector,
            string catalogName,
            string parameterName)
            where TId : struct, IIdentifier
        {
            if (TryValidateIds(entries, idSelector, out var error))
                return;

            throw new ArgumentException($"{catalogName} {error}", parameterName);
        }

        private static bool TryValidateIds<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> idSelector,
            out string error)
            where TId : struct, IIdentifier
        {
            if (idSelector == null)
                throw new ArgumentNullException(nameof(idSelector));

            var usedIds = new HashSet<TId>();
            var index = 0;

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    if (entry is null)
                    {
                        error = $"contains a null entry at index {index}.";
                        return false;
                    }

                    var id = idSelector(entry);

                    if (id.IsEmpty)
                    {
                        error = $"contains an empty ID at index {index}.";
                        return false;
                    }

                    if (usedIds.Add(id) is false)
                    {
                        error = $"contains duplicate ID: {id}.";
                        return false;
                    }

                    index++;
                }
            }

            error = string.Empty;
            return true;
        }
    }
}