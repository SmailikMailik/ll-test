using System;
using LL.Identifiers;

namespace LL.Game.Currencies
{
    internal readonly struct CurrencyId : IEquatable<CurrencyId>
    {
        private readonly StringId _value;

        internal string Value => _value.Value;
        internal bool IsEmpty => _value.IsEmpty;

        internal CurrencyId(string value)
        {
            _value = new StringId(value);
        }

        public bool Equals(CurrencyId other)
        {
            return _value.Equals(other._value);
        }

        public override bool Equals(object obj)
        {
            return obj is CurrencyId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString() => _value.ToString();

        public static bool operator ==(CurrencyId left, CurrencyId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CurrencyId left, CurrencyId right)
        {
            return left.Equals(right) is false;
        }
    }
}