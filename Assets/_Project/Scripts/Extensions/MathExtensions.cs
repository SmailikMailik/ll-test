using UnityEngine;

namespace LL.Extensions
{
    internal static class MathExtensions
    {
        internal static int Clamp(this int value, int min, int max) => value switch
        {
            _ when value < min => min,
            _ when value > max => max,
            _ => value
        };

        internal static int LoopClamp(this int value, int min, int max) => value switch
        {
            _ when value < min => max,
            _ when value > max => min,
            _ => value
        };

        internal static float Clamp(this float value, float min, float max) => value switch
        {
            _ when value < min => min,
            _ when value > max => max,
            _ => value
        };

        internal static float LoopClamp(this float value, float min, float max) => value switch
        {
            _ when value < min => max,
            _ when value > max => min,
            _ => value
        };

        internal static float Abs(this float value) => Mathf.Abs(value);

        internal static float Normalize(this float value, float max) => value.Normalize(0f, max);
        internal static float Normalize(this float value, float min, float max) => (value - min) / (max - min);

        internal static float DeNormalize(this float value, float max) => value.DeNormalize(0f, max);
        internal static float DeNormalize(this float value, float min, float max) => value * (max - min) + min;

        internal static float ReScale(this float value, float maxFrom, float maxTo) => value.Normalize(maxFrom).DeNormalize(maxTo);
        internal static float ReScale(this float value, float minFrom, float maxFrom, float minTo, float maxTo) => value.Normalize(minFrom, maxFrom).DeNormalize(minTo, maxTo);

        internal static float ToPercent(this float value, float max) => value.Normalize(max) * 100f;
    }
}