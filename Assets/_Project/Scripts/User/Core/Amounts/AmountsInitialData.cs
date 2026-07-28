using System;
using System.Collections.Generic;
using System.Linq;
using LL.Identifiers;

namespace LL.User.Core.Amounts
{
    internal sealed class AmountsInitialData<TId>
        where TId : struct, IIdentifier
    {
        internal IReadOnlyList<Amount<TId>> Amounts { get; }

        internal AmountsInitialData(IEnumerable<Amount<TId>> entries)
        {
            var amounts = new Dictionary<TId, long>();

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    if (entry.Id.IsEmpty)
                        continue;

                    amounts.TryGetValue(entry.Id, out var current);
                    amounts[entry.Id] = Math.Min(current + entry.Value, int.MaxValue);
                }
            }

            var copy = amounts.Select(pair => new Amount<TId>(pair.Key, (int)pair.Value)).ToArray();
            Amounts = Array.AsReadOnly(copy);
        }
    }
}