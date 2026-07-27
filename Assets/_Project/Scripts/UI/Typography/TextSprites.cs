namespace LL.UI.Typography
{
    internal enum TextSprite : byte
    {
        Cash = 0,
        Gold = 1,
        MasterPoints = 2,
        Max = 3
    }

    internal static class TextSprites
    {
        internal static string GetName(TextSprite sprite) => sprite switch
        {
            TextSprite.Cash => "cash",
            TextSprite.Gold => "gold",
            TextSprite.MasterPoints => "master_points",
            TextSprite.Max => "max",
            _ => string.Empty
        };
    }
}