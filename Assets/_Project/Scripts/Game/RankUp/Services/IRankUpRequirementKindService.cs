using LL.Game.Heroes;
using LL.Game.Ranks;

namespace LL.Game.RankUp.Services
{
    internal interface IRankUpRequirementKindService
    {
        bool Supports(RankUpRequirementDefinition requirement);

        bool IsSatisfied(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement);

        bool TryActivate(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement);

        bool TryCommit(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement);

        bool TryRollback(RankUpRequirementDefinition requirement);
    }
}