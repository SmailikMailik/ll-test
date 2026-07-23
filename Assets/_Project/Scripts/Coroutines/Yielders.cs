using System.Collections.Generic;
using UnityEngine;

namespace LL.Coroutines
{
    internal static class Yielders
    {
        private class FloatComparer : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float x, float y) => x == y;
            int IEqualityComparer<float>.GetHashCode(float obj) => obj.GetHashCode();
        }

        private static readonly Dictionary<float, WaitForSeconds> _waitForSeconds =
            new(100, new FloatComparer());

        internal static WaitForEndOfFrame EndOfFrame { get; } = new();
        internal static WaitForFixedUpdate FixedUpdate { get; } = new();

        internal static WaitForSeconds WaitForSeconds(float seconds)
        {
            if (_waitForSeconds.TryGetValue(seconds, out var waitForSeconds) is false)
                _waitForSeconds.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));

            return waitForSeconds;
        }
    }
}