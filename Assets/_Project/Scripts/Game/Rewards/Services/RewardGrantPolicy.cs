using System;
using LL.User.State.Rewards;
using VContainer;

namespace LL.Game.Rewards.Services
{
    internal sealed class RewardGrantPolicy
    {
        private readonly IUserRewardClaims _claims;

        [Inject]
        internal RewardGrantPolicy(IUserRewardClaims claims)
        {
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
        }

        internal bool CanGrant(RewardBundle bundle)
        {
            if (bundle == null)
                throw new ArgumentNullException(nameof(bundle));

            return RequiresClaim(bundle.GrantMode) is false ||
                   _claims.Contains(bundle.Id) is false;
        }

        internal bool TryRegisterGrant(RewardBundle bundle)
        {
            if (bundle == null)
                throw new ArgumentNullException(nameof(bundle));

            return RequiresClaim(bundle.GrantMode) is false ||
                   _claims.TryClaim(bundle.Id);
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