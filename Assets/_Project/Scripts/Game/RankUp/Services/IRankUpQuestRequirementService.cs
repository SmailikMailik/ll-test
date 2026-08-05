using LL.Game.Heroes;
using LL.Game.Ranks;

namespace LL.Game.RankUp.Services
{
    internal interface IRankUpQuestRequirementService
    {
        bool TryStart(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            QuestRankUpRequirementDefinition quest);

        bool TryCompleteForTesting(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            QuestRankUpRequirementDefinition quest);

        bool TryExpire(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            QuestRankUpRequirementDefinition quest);

        void Clear(HeroId heroId, RankId rankId, RankUpOptionId optionId);
    }
}