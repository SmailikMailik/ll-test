using System;
using System.Collections.Generic;
using LL.Game.Identifiers;
using LL.Game.Items;

namespace LLEditor.Validation.References.Items
{
    internal static class ItemReferenceIdCollector
    {
        internal static bool TryCollect<TEntry>(
            IEnumerable<TEntry> entries,
            Func<TEntry, ItemId> getId,
            out HashSet<ItemId> ids)
            where TEntry : class
        {
            if (getId == null)
                throw new ArgumentNullException(nameof(getId));

            if (entries == null)
            {
                ids = null;
                return false;
            }

            ids = new HashSet<ItemId>();

            foreach (var entry in entries)
            {
                if (entry == null)
                    continue;

                var id = getId(entry);

                if (IdentifierValidator.IsValid(id))
                    ids.Add(id);
            }

            return true;
        }
    }
}