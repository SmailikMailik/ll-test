using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Rewards.Services;
using LL.User.State;
using LL.User.State.Heroes;
using UnityEngine;
using VContainer;

namespace LL.Game.RankUp.Services
{
    internal sealed class RankUpService : IRankUpService
    {
        private readonly RankUpCatalog _catalog;
        private readonly IUserHeroProgress _progress;
        private readonly IUserHeroProgressCommands _progressCommands;
        private readonly IRankUpRequirementService _requirements;
        private readonly IRewardGrantService _rewardGrantService;
        private readonly IUserStateChangeBatch _changeBatch;

        [Inject]
        internal RankUpService(
            RankUpCatalog catalog,
            IUserHeroProgress progress,
            IUserHeroProgressCommands progressCommands,
            IRankUpRequirementService requirements,
            IRewardGrantService rewardGrantService,
            IUserStateChangeBatch changeBatch)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _progressCommands = progressCommands ?? throw new ArgumentNullException(nameof(progressCommands));
            _requirements = requirements ?? throw new ArgumentNullException(nameof(requirements));
            _rewardGrantService = rewardGrantService ?? throw new ArgumentNullException(nameof(rewardGrantService));
            _changeBatch = changeBatch ?? throw new ArgumentNullException(nameof(changeBatch));
        }

        public bool TryGetDefinition(HeroId heroId, out RankUpDefinition definition)
        {
            definition = null;
            return _progress.TryGetProgress(heroId, out var progress) &&
                   _catalog.TryGetDefinition(heroId, progress.RankId, out definition);
        }

        public bool TryActivateOption(HeroId heroId, RankUpOptionId optionId)
        {
            if (TryGetOption(heroId, optionId, out var definition, out var option) is false ||
                _progress.CanRankUp(heroId) is false)
            {
                return false;
            }

            foreach (var requirement in option.Requirements)
            {
                if (_requirements.TryActivate(heroId, definition.RankId, option.Id, requirement) is false)
                    return false;
            }

            return true;
        }

        public bool CanCompleteOption(HeroId heroId, RankUpOptionId optionId)
        {
            if (TryGetOption(heroId, optionId, out var definition, out var option) is false ||
                _progress.CanRankUp(heroId) is false ||
                _rewardGrantService.CanGrant(definition.RewardId) is false)
            {
                return false;
            }

            foreach (var requirement in option.Requirements)
            {
                if (_requirements.IsSatisfied(heroId, definition.RankId, option.Id, requirement) is false)
                    return false;
            }

            return true;
        }

        public bool TryCompleteOption(
            HeroId heroId,
            RankUpOptionId optionId,
            out IReadOnlyList<ItemAmount> rewardItems)
        {
            IReadOnlyList<ItemAmount> appliedRewardItems = Array.Empty<ItemAmount>();
            var succeeded = _changeBatch.Execute(() => TryCompleteOptionCore(heroId, optionId, out appliedRewardItems));
            rewardItems = appliedRewardItems;
            return succeeded;
        }

        private bool TryCompleteOptionCore(
            HeroId heroId,
            RankUpOptionId optionId,
            out IReadOnlyList<ItemAmount> rewardItems)
        {
            rewardItems = Array.Empty<ItemAmount>();

            if (TryGetOption(heroId, optionId, out var definition, out var option) is false ||
                CanCompleteOption(heroId, optionId) is false)
            {
                return false;
            }

            var committed = new List<RankUpRequirementDefinition>(option.Requirements.Count);

            foreach (var requirement in option.Requirements)
            {
                if (_requirements.TryCommit(heroId, definition.RankId, option.Id, requirement))
                {
                    committed.Add(requirement);
                    continue;
                }

                Rollback(committed);
                return false;
            }

            if (_progressCommands.TryRankUp(heroId) is false)
            {
                Rollback(committed);
                return false;
            }

            if (_rewardGrantService.TryGrant(definition.RewardId, out rewardItems))
                return true;

            Debug.LogError($"Failed to grant rank-up reward: {definition.RewardId}");
            return true;
        }

        private bool TryGetOption(
            HeroId heroId,
            RankUpOptionId optionId,
            out RankUpDefinition definition,
            out RankUpOptionDefinition option)
        {
            option = null;
            return TryGetDefinition(heroId, out definition) &&
                   definition.TryGetOption(optionId, out option);
        }

        private void Rollback(IReadOnlyList<RankUpRequirementDefinition> requirements)
        {
            for (var index = requirements.Count - 1; index >= 0; index--)
            {
                if (_requirements.TryRollback(requirements[index]) is false)
                    Debug.LogError($"Failed to roll back rank-up requirement: {requirements[index].Id}");
            }
        }
    }
}