namespace LL.UI.Typography
{
    internal enum TextStyle : byte
    {
        Accent = 0,
        Gold = 1,
        Muted = 2,
        Positive = 3,
        White = 4
    }

    internal static class TextStyles
    {
        internal static string GetName(TextStyle style) => style switch
        {
            TextStyle.Accent => "Accent",
            TextStyle.Gold => "Gold",
            TextStyle.Muted => "Muted",
            TextStyle.Positive => "Positive",
            TextStyle.White => "White",
            _ => string.Empty
        };
    }
}