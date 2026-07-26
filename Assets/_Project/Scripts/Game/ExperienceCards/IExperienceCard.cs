namespace LL.Game.ExperienceCards
{
    internal interface IExperienceCard
    {
        ExperienceCardId Id { get; }
        int ExperienceAmount { get; }
    }
}