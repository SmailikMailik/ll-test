using System;

namespace LL
{
    internal sealed class ReactiveParameter<T>
    {
        internal event Action<T> Changed;

        internal T DefaultValue { get; }

        internal T Value
        {
            get => _value;
            set
            {
                if (_value is not null && _value.Equals(value))
                    return;

                _value = value;
                Changed?.Invoke(_value);
            }
        }

        private T _value;

        internal ReactiveParameter(T defaultValue = default)
        {
            DefaultValue = defaultValue;
            _value = defaultValue;
        }
    }
}