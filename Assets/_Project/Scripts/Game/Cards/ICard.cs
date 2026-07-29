using LL.Game.Items;

namespace LL.Game.Cards
{
    internal interface ICard
    {
        ItemId Id { get; }
        int ExperienceAmount { get; }
    }
}