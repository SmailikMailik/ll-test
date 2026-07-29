using System;
using System.Collections.Generic;
using System.Linq;
using VContainer;

namespace LL.Game.Rewards.Services
{
    internal sealed class RewardGrantService : IRewardGrantService
    {
        private readonly RewardBundleCatalog _catalog;
        private readonly IRewardService _rewardService;
        private readonly RewardGrantPolicy _grantPolicy;

        [Inject]
        internal RewardGrantService(
            RewardBundleCatalog catalog,
            IRewardService rewardService,
            RewardGrantPolicy grantPolicy)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
            _grantPolicy = grantPolicy ?? throw new ArgumentNullException(nameof(grantPolicy));
        }

        public bool CanGrant(RewardBundleId id)
        {
            return _catalog.TryGetBundle(id, out var bundle) &&
                   CanGrant(bundle);
        }

        public bool TryGrant(RewardBundleId id, out IReadOnlyList<IReward> rewards)
        {
            rewards = Array.Empty<IReward>();

            if (_catalog.TryGetBundle(id, out var bundle) is false ||
                CanGrant(bundle) is false)
            {
                return false;
            }

            foreach (var reward in bundle.Rewards)
            {
                if (_rewardService.TryApply(reward) is false)
                    return false;
            }

            if (_grantPolicy.TryRegisterGrant(bundle) is false)
                return false;

            rewards = bundle.Rewards;
            return true;
        }

        private bool CanGrant(RewardBundle bundle)
        {
            return _grantPolicy.CanGrant(bundle) &&
                   bundle.Rewards.All(_rewardService.CanApply);
        }
    }
}