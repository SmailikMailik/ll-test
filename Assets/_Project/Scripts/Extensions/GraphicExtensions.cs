using UnityEngine.UI;

namespace LL.Extensions
{
    internal static class GraphicExtensions
    {
        internal static void SetAlpha(this Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}