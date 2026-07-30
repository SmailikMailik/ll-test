using System;
using LL.Game.Identifiers;

namespace LL.Game.Quests
{
    internal readonly struct QuestId : IIdentifier, IEquatable<QuestId>
    {
        public string Value { get; }

        internal QuestId(string value) => Value = value;

        public bool Equals(QuestId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is QuestId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}