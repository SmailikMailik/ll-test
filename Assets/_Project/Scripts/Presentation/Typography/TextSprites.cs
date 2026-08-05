namespace LL.Presentation.Typography
{
    internal static class TextSprites
    {
        internal static string GetName(TextSprite sprite) => sprite switch
        {
            TextSprite.Cash => "cash",
            TextSprite.Gold => "gold",
            TextSprite.MasterPoints => "master_points",
            TextSprite.Max => "max",
            TextSprite.Time => "time",
            TextSprite.Link => "link",
            _ => string.Empty
        };
    }
}