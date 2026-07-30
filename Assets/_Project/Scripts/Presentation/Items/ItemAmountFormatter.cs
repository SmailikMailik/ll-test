using System.Collections.Generic;
using LL.Game.Items;
using LL.Presentation.Typography;

namespace LL.Presentation.Items
{
    internal static class ItemAmountFormatter
    {
        private static readonly Dictionary<ItemId, (TextSprite Sprite, TextStyle Style)> _formats = new()
        {
            [ItemIds.Soft] = (TextSprite.Cash, TextStyle.White),
            [ItemIds.Hard] = (TextSprite.Gold, TextStyle.Gold),
            [ItemIds.MasterPoint] = (TextSprite.MasterPoints, TextStyle.White)
        };

        internal static string Format(ItemId id, int amount)
        {
            var formattedAmount = TextFormatter.Number(amount);

            if (_formats.TryGetValue(id, out var format) is false)
                return formattedAmount;

            var sprite = TextTags.Sprite(format.Sprite);
            var separator = TextSymbols.GetValue(TextSymbol.NonBreakingSpace);
            return TextTags.Style($"{sprite}{separator}{formattedAmount}", format.Style);
        }
    }
}