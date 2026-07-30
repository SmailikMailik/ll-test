namespace LL.Presentation.Typography
{
    internal static class TextTags
    {
        private const string StyleMask = "<style=\"{0}\">{1}</style>";
        private const string SpriteMask = "<sprite name=\"{0}\">";

        internal static string Style(string text, TextStyle style) =>
            string.Format(StyleMask, TextStyles.GetName(style), text);

        internal static string Sprite(TextSprite sprite) =>
            string.Format(SpriteMask, TextSprites.GetName(sprite));
    }
}