using System;
using System.Collections.Generic;

namespace LL.Game.Identifiers
{
    internal static class IdentifierCollectionValidator
    {
        internal static bool IsValid<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId)
            where TId : struct, IIdentifier
        {
            return TryValidate(entries, getId, out _);
        }

        internal static void Validate<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            string parameterName)
            where TId : struct, IIdentifier
        {
            if (TryValidate(entries, getId, out var error))
                return;

            throw new ArgumentException(error, parameterName);
        }

        private static bool TryValidate<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            out string error)
            where TId : struct, IIdentifier
        {
            if (getId == null)
                throw new ArgumentNullException(nameof(getId));

            var usedIds = new HashSet<TId>();
            var index = 0;

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    if (entry is null)
                    {
                        error = $"Collection contains a null entry at index {index}.";
                        return false;
                    }

                    var id = getId(entry);

                    if (id.IsEmpty)
                    {
                        error = $"Collection contains an empty ID at index {index}.";
                        return false;
                    }

                    if (usedIds.Add(id) is false)
                    {
                        error = $"Collection contains duplicate ID: {id}.";
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