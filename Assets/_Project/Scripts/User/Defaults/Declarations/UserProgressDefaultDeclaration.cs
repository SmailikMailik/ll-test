namespace LL.User.Defaults.Declarations
{
    internal sealed class UserProgressDefaultDeclaration
    {
        internal string RankId { get; }
        internal int Experience { get; }

        internal UserProgressDefaultDeclaration(string rankId, int experience)
        {
            RankId = rankId;
            Experience = experience;
        }
    }
}