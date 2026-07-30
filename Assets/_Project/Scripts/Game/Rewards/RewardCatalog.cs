using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;

namespace LL.Game.Rewards
{
    internal sealed class RewardCatalog
    {
        private readonly IReadOnlyDictionary<RewardId, Reward> _rewardsById;

        internal RewardCatalog(IEnumerable<Reward> rewards)
        {
            var entries = rewards?.ToArray() ?? Array.Empty<Reward>();
            IdentifierCollectionValidator.EnsureValid(
                entries,
                reward => reward.Id,
                nameof(rewards));

            _rewardsById = entries.ToDictionary(reward => reward.Id);
        }

        internal bool TryGetReward(RewardId id, out Reward reward) =>
            _rewardsById.TryGetValue(id, out reward);
    }
}