using System.Collections.Generic;
using LL.Infrastructure.Collections;

namespace LL.Game.Data.Declarations
{
    internal sealed class RankUpOptionDeclaration
    {
        internal string OptionId { get; }
        internal IReadOnlyList<RankUpRequirementDeclaration> Requirements { get; }

        internal RankUpOptionDeclaration(
            string optionId,
            IEnumerable<RankUpRequirementDeclaration> requirements)
        {
            OptionId = optionId;
            Requirements = requirements.ToReadOnlyCopy();
        }
    }
}