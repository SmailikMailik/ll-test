using System;
using LL.Game.Heroes;
using LL.Game.Ranks;
using LL.Game.RankUp;

namespace LL.User.State.RankUp
{
    internal interface IUserRankUpAttemptsCommands
    {
        bool TryStartQuest(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementId requirementId,
            TimeSpan duration);

        bool TryAddQuestProgress(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementId requirementId,
            int amount);

        bool TryExpireQuest(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementId requirementId);

        void ClearOption(HeroId heroId, RankId rankId, RankUpOptionId optionId);
        void ClearRank(HeroId heroId, RankId rankId);
    }
}