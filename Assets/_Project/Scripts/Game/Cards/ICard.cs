namespace LL.Game.Cards
{
    internal interface ICard
    {
        CardId Id { get; }
        int ExperienceAmount { get; }
    }
}