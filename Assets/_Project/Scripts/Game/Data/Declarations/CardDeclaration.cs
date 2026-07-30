namespace LL.Game.Data.Declarations
{
    internal sealed class CardDeclaration
    {
        internal string Id { get; }
        internal int ExperienceAmount { get; }

        internal CardDeclaration(string id, int experienceAmount)
        {
            Id = id;
            ExperienceAmount = experienceAmount;
        }
    }
}