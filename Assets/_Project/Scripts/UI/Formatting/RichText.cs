namespace LL.UI.Formatting
{
    internal static class RichText
    {
        private const string StyleMask = "<style=\"{0}\">{1}</style>";
        private const string SpriteMask = "<sprite name=\"{0}\">";

        internal static string Style(string text, string style) => string.Format(StyleMask, style, text);
        internal static string Sprite(string name) => string.Format(SpriteMask, name);
    }
}