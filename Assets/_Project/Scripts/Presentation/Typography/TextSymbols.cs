namespace LL.Presentation.Typography
{
    internal enum TextSymbol : byte
    {
        NonBreakingSpace = 0
    }

    internal static class TextSymbols
    {
        internal static string GetValue(TextSymbol symbol) => symbol switch
        {
            TextSymbol.NonBreakingSpace => "\u00A0",
            _ => string.Empty
        };
    }
}