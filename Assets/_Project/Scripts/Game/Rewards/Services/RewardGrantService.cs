using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.User.State.Items;
using VContainer;

namespace LL.Game.Rewards.Services
{
    internal sealed class RewardGrantService : IRewardGrantService
    {
        private readonly RewardCatalog _catalog;
        private readonly IUserItems _items;

        [Inject]
        internal RewardGrantService(
            RewardCatalog catalog,
            IUserItems items)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public bool CanGrant(RewardId id)
        {
            return _catalog.TryGetReward(id, out var reward) &&
                   CanGrant(reward);
        }

        public bool TryGrant(RewardId id, out IReadOnlyList<ItemAmount> items)
        {
            items = Array.Empty<ItemAmount>();

            if (_catalog.TryGetReward(id, out var reward) is false ||
                CanGrant(reward) is false)
            {
                return false;
            }

            var appliedCount = 0;

            foreach (var item in reward.Items)
            {
                if (_items.TryAdd(item.Id, item.Amount) is false)
                {
                    Rollback(reward.Items, appliedCount);
                    return false;
                }

                appliedCount++;
            }

            items = reward.Items;
            return true;
        }

        private bool CanGrant(Reward reward)
        {
            return reward.Items.All(item => _items.CanAdd(item.Id, item.Amount));
        }

        private void Rollback(IReadOnlyList<ItemAmount> items, int appliedCount)
        {
            for (var index = appliedCount - 1; index >= 0; index--)
            {
                var item = items[index];

                if (_items.TrySpend(item.Id, item.Amount) is false)
                    throw new InvalidOperationException($"Failed to roll back granted item: {item.Id}.");
            }
        }
    }
}