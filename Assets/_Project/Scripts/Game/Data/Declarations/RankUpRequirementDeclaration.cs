namespace LL.Game.Data.Declarations
{
    internal abstract class RankUpRequirementDeclaration
    {
        internal string RequirementId { get; }

        protected RankUpRequirementDeclaration(string requirementId)
        {
            RequirementId = requirementId;
        }
    }
}