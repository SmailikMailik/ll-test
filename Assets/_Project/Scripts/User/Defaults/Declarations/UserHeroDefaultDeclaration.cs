namespace LL.User.Defaults.Declarations
{
    internal sealed class UserHeroDefaultDeclaration
    {
        internal string HeroId { get; }
        internal string RankId { get; }
        internal int Experience { get; }

        internal UserHeroDefaultDeclaration(
            string heroId,
            string rankId,
            int experience)
        {
            HeroId = heroId;
            RankId = rankId;
            Experience = experience;
        }
    }
}