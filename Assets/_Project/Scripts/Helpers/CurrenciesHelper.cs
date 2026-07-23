using System.Collections.Generic;
using UnityEngine;

namespace LL.Helpers
{
    internal enum CurrencyType : byte
    {
        None = 0,
        Soft = 1,
        Hard = 2,
    }

    internal static class CurrenciesHelper
    {
        private static readonly Dictionary<CurrencyType, int> _currencies = new()
        {
            //[CurrencyType.Soft] = UserData.SoftAmount
            //[CurrencyType.Hard] = UserData.HardAmount
        };

        internal static bool TrySpend(CurrencyType type, int value)
        {
            if (_currencies.TryGetValue(type, out var currency) is false)
            {
                Debug.LogError($"[CurrenciesHelper::TrySpend] Currency type '{type}' not defined!");
                return false;
            }

            if (value > currency)
            {
                Debug.Log($"[CurrenciesHelper::TrySpend] Currency type '{type}' not enough!");
                return false;
            }

            currency -= value;
            return true;
        }

        internal static bool TryAdd(CurrencyType type, int value)
        {
            if (_currencies.TryGetValue(type, out var currency) is false)
            {
                Debug.LogError($"[CurrenciesHelper::TryAdd] Currency type '{type}' not defined!");
                return false;
            }

            currency += value;
            return true;
        }
    }
}