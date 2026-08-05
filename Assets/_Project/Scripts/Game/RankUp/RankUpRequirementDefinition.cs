using LL.Game.Identifiers;

namespace LL.Game.RankUp
{
    internal abstract class RankUpRequirementDefinition
    {
        internal RankUpRequirementId Id { get; }

        protected RankUpRequirementDefinition(RankUpRequirementId id)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));
            Id = id;
        }
    }
}