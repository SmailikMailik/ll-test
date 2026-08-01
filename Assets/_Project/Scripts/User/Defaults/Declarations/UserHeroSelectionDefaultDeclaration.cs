namespace LL.User.Defaults.Declarations
{
    internal sealed class UserHeroSelectionDefaultDeclaration
    {
        internal string HeroId { get; }

        internal UserHeroSelectionDefaultDeclaration(string heroId)
        {
            HeroId = heroId;
        }
    }
}