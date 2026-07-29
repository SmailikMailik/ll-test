using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Identifiers;

namespace LL.Game.Rewards
{
    internal sealed class RewardCatalog
    {
        private readonly IReadOnlyDictionary<RewardId, Reward> _rewards;

        internal RewardCatalog(IEnumerable<Reward> rewards)
        {
            var copy = rewards?.ToArray() ?? Array.Empty<Reward>();

            IdentifierCollectionValidator.EnsureValid(
                copy,
                reward => reward.Id,
                nameof(rewards));

            _rewards = copy.ToDictionary(reward => reward.Id);
        }

        internal bool TryGetReward(RewardId id, out Reward reward)
        {
            return _rewards.TryGetValue(id, out reward);
        }
    }
}