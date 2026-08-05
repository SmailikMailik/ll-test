namespace LL.Presentation.Typography
{
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