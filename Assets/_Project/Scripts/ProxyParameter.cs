using System;

namespace LL
{
    internal sealed class ProxyParameter<T>
    {
        private readonly Func<T> _getted;
        private readonly Action<T> _setted;

        internal ProxyParameter(Func<T> getted, Action<T> setted)
        {
            _getted = getted;
            _setted = setted;
        }

        internal T GetValue() => _getted.Invoke();
        internal void SetValue(T value) => _setted.Invoke(value);
    }
}