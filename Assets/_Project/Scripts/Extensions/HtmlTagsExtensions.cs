using UnityEngine;

namespace LL.Extensions
{
    internal static class HtmlTagsExtensions
    {
        private const string ColorMask = "<color={0}>{1}</color>";
        private const string SizeMask = "<size={0}>{1}</size>";
        private const string BoldMask = "<b>{0}</b>";

        internal static string Colorize(this object text, Color color) =>
            text.Colorize($"#{ColorUtility.ToHtmlStringRGB(color)}");

        internal static string Colorize(this object text, string color) =>
            string.Format(ColorMask, color, text);

        internal static string ReSize(this object text, int size) =>
            string.Format(SizeMask, size, text);

        internal static string MakeBold(this object text) =>
            string.Format(BoldMask, text);
    }
}