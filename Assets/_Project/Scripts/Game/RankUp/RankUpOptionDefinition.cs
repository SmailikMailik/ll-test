using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;

namespace LL.Game.RankUp
{
    internal sealed class RankUpOptionDefinition
    {
        internal RankUpOptionId Id { get; }
        internal IReadOnlyList<RankUpRequirementDefinition> Requirements { get; }

        internal RankUpOptionDefinition(
            RankUpOptionId id,
            IEnumerable<RankUpRequirementDefinition> requirements)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));

            var requirementArray = requirements?.ToArray() ?? Array.Empty<RankUpRequirementDefinition>();
            IdentifierCollectionValidator.EnsureValid(
                requirementArray,
                requirement => requirement.Id,
                nameof(requirements));

            Id = id;
            Requirements = Array.AsReadOnly(requirementArray);
        }
    }
}