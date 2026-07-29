using System;
using System.Collections.Generic;
using System.Linq;
using LL.Rewards.Models;
using LL.User.Core.Rewards;
using VContainer;

namespace LL.Rewards.Services
{
    internal sealed class RewardGrantService : IRewardGrantService
    {
        private readonly RewardBundleCatalog _catalog;
        private readonly IRewardService _rewardService;
        private readonly IUserRewardClaims _claims;

        [Inject]
        internal RewardGrantService(
            RewardBundleCatalog catalog,
            IRewardService rewardService,
            IUserRewardClaims claims)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _rewardService = rewardService ?? throw new ArgumentNullException(nameof(rewardService));
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
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

            if (bundle.GrantMode == RewardGrantMode.Once && _claims.TryClaim(id) is false)
                return false;

            rewards = bundle.Rewards;
            return true;
        }

        private bool CanGrant(RewardBundle bundle)
        {
            return (bundle.GrantMode != RewardGrantMode.Once || _claims.Contains(bundle.Id) is false) &&
                   bundle.Rewards.All(_rewardService.CanApply);
        }
    }
}