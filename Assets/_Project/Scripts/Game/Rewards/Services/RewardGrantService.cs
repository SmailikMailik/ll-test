using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.User.State.Items;
using LL.User.State.Rewards;
using VContainer;

namespace LL.Game.Rewards.Services
{
    internal sealed class RewardGrantService : IRewardGrantService
    {
        private readonly RewardCatalog _catalog;
        private readonly IUserItems _items;
        private readonly IUserRewardClaims _claims;

        [Inject]
        internal RewardGrantService(
            RewardCatalog catalog,
            IUserItems items,
            IUserRewardClaims claims)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
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

            if (TryRegisterGrant(reward) is false)
            {
                Rollback(reward.Items, appliedCount);
                return false;
            }

            items = reward.Items;
            return true;
        }

        private bool CanGrant(Reward reward)
        {
            return CanRegisterGrant(reward) &&
                   reward.Items.All(item => _items.CanAdd(item.Id, item.Amount));
        }

        private bool CanRegisterGrant(Reward reward)
        {
            return RequiresClaim(reward.GrantMode) is false ||
                   _claims.Contains(reward.Id) is false;
        }

        private bool TryRegisterGrant(Reward reward)
        {
            return RequiresClaim(reward.GrantMode) is false ||
                   _claims.TryClaim(reward.Id);
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

        private static bool RequiresClaim(RewardGrantMode grantMode) => grantMode switch
        {
            RewardGrantMode.Once => true,
            RewardGrantMode.Repeatable => false,
            _ => throw new ArgumentOutOfRangeException(
                nameof(grantMode),
                grantMode,
                "Reward grant mode is not supported.")
        };
    }
}