using System;
using LL.Identifiers;

namespace LL.User.Core.Amounts
{
    internal readonly struct Amount<TId>
        where TId : struct, IIdentifier
    {
        internal TId Id { get; }
        internal int Value { get; }

        internal Amount(TId id, int value)
        {
            Id = id;
            Value = Math.Max(0, value);
        }
    }
}