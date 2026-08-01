using System;
using LL.Game.Heroes;
using LL.Game.Ranks;
using LL.User.State.RankUp;
using VContainer;

namespace LL.Game.RankUp.Services
{
    internal sealed class QuestRankUpRequirementService :
        IRankUpRequirementKindService,
        IRankUpQuestRequirementService
    {
        private readonly IUserRankUpAttempts _attempts;
        private readonly IUserRankUpAttemptsCommands _attemptCommands;

        [Inject]
        internal QuestRankUpRequirementService(
            IUserRankUpAttempts attempts,
            IUserRankUpAttemptsCommands attemptCommands)
        {
            _attempts = attempts ?? throw new ArgumentNullException(nameof(attempts));
            _attemptCommands = attemptCommands ?? throw new ArgumentNullException(nameof(attemptCommands));
        }

        public bool Supports(RankUpRequirementDefinition requirement)
        {
            return requirement is QuestRankUpRequirementDefinition;
        }

        public bool IsSatisfied(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            return requirement is QuestRankUpRequirementDefinition quest &&
                   IsQuestSatisfied(heroId, rankId, optionId, quest);
        }

        public bool TryActivate(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            return requirement is QuestRankUpRequirementDefinition quest &&
                   TryStart(heroId, rankId, optionId, quest);
        }

        public bool TryCommit(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            RankUpRequirementDefinition requirement)
        {
            return IsSatisfied(heroId, rankId, optionId, requirement);
        }

        public bool TryRollback(RankUpRequirementDefinition requirement)
        {
            return requirement is QuestRankUpRequirementDefinition;
        }

        public bool TryStart(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            QuestRankUpRequirementDefinition quest)
        {
            if (quest is null)
                throw new ArgumentNullException(nameof(quest));

            if (_attempts.TryGetQuest(heroId, rankId, optionId, quest.Id, out _))
                return true;

            return _attemptCommands.TryStartQuest(
                heroId,
                rankId,
                optionId,
                quest.Id,
                quest.Duration);
        }

        public bool TryCompleteForTesting(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            QuestRankUpRequirementDefinition quest)
        {
            if (quest is null)
                throw new ArgumentNullException(nameof(quest));

            if (_attempts.TryGetQuest(heroId, rankId, optionId, quest.Id, out var state) is false)
                return false;

            var remainingCount = Math.Max(0, quest.RequiredCount - state.CurrentCount);
            return remainingCount == 0 ||
                   _attemptCommands.TryAddQuestProgress(heroId, rankId, optionId, quest.Id, remainingCount);
        }

        public bool TryExpire(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            QuestRankUpRequirementDefinition quest)
        {
            if (quest is null)
                throw new ArgumentNullException(nameof(quest));

            if (IsQuestSatisfied(heroId, rankId, optionId, quest))
                return false;

            return _attemptCommands.TryExpireQuest(heroId, rankId, optionId, quest.Id);
        }

        public void Clear(HeroId heroId, RankId rankId, RankUpOptionId optionId)
        {
            _attemptCommands.ClearOption(heroId, rankId, optionId);
        }

        private bool IsQuestSatisfied(
            HeroId heroId,
            RankId rankId,
            RankUpOptionId optionId,
            QuestRankUpRequirementDefinition quest)
        {
            return _attempts.TryGetQuest(heroId, rankId, optionId, quest.Id, out var state) &&
                   state.CurrentCount >= quest.RequiredCount;
        }
    }
}