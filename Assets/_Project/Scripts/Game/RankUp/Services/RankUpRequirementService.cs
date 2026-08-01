using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Ranks;
using VContainer;

namespace LL.Game.RankUp.Services
{
    internal sealed class RankUpRequirementService : IRankUpRequirementService
    {
        private readonly IReadOnlyList<IRankUpRequirementKindService> _services;

        [Inject]
        internal RankUpRequirementService(IReadOnlyList<IRankUpRequirementKindService> services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public bool IsSatisfied(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            if (requirement is null)
                throw new ArgumentNullException(nameof(requirement));

            return TryGetService(requirement, out var service) &&
                   service.IsSatisfied(heroId, rankId, optionId, requirement);
        }

        public bool TryActivate(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            if (requirement is null)
                throw new ArgumentNullException(nameof(requirement));

            return TryGetService(requirement, out var service) &&
                   service.TryActivate(heroId, rankId, optionId, requirement);
        }

        public bool TryCommit(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            if (requirement is null)
                throw new ArgumentNullException(nameof(requirement));

            return TryGetService(requirement, out var service) &&
                   service.TryCommit(heroId, rankId, optionId, requirement);
        }

        public bool TryRollback(RankUpRequirementDefinition requirement)
        {
            if (requirement is null)
                throw new ArgumentNullException(nameof(requirement));

            return TryGetService(requirement, out var service) && service.TryRollback(requirement);
        }

        private bool TryGetService(
            RankUpRequirementDefinition requirement,
            out IRankUpRequirementKindService service)
        {
            foreach (var candidate in _services)
            {
                if (candidate.Supports(requirement))
                {
                    service = candidate;
                    return true;
                }
            }

            service = null;
            return false;
        }
    }
}