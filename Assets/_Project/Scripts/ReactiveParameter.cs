using System;

namespace LL
{
    internal interface IReadOnlyReactiveParameter<out T>
    {
        event Action<T> Changed;

        T DefaultValue { get; }
        T Value { get; }
    }

    internal sealed class ReactiveParameter<T> : IReadOnlyReactiveParameter<T>
    {
        public event Action<T> Changed;

        public T DefaultValue { get; }

        public T Value
        {
            get => _value;
            set => SetValue(value);
        }

        private T _value;

        internal ReactiveParameter(T defaultValue = default)
        {
            DefaultValue = defaultValue;
            _value = defaultValue;
        }

        internal void SetValue(T value, bool forceNotify = false)
        {
            if (forceNotify is false && Equals(_value, value))
                return;

            _value = value;
            Changed?.Invoke(_value);
        }
    }
}