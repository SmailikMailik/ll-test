using System;
using LL.Game.Identifiers;

namespace LL.Game.Promotions
{
    internal readonly struct PromotionRequirementId : IIdentifier, IEquatable<PromotionRequirementId>
    {
        public string Value { get; }

        internal PromotionRequirementId(string value) => Value = value;

        public bool Equals(PromotionRequirementId other) => StringComparer.Ordinal.Equals(Value, other.Value);

        public override bool Equals(object obj) => obj is PromotionRequirementId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }
}