using System;
using UnityEngine;

namespace LL.UI.VisualStates
{
    [Serializable]
    internal struct VisualStateId : IEquatable<VisualStateId>
    {
        [SerializeField] private string _value;

        internal bool IsEmpty => string.IsNullOrEmpty(_value);

        private VisualStateId(string value)
        {
            _value = value;
        }

        internal static VisualStateId Create() => new(Guid.NewGuid().ToString("N"));

        public bool Equals(VisualStateId other) => string.Equals(_value, other._value, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is VisualStateId other && Equals(other);
        public override int GetHashCode() => _value == null ? 0 : StringComparer.Ordinal.GetHashCode(_value);
        public override string ToString() => _value ?? string.Empty;

        public static bool operator ==(VisualStateId left, VisualStateId right) => left.Equals(right);
        public static bool operator !=(VisualStateId left, VisualStateId right) => left.Equals(right) is false;
    }
}