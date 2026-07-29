using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Rewards.Configuration
{
    internal sealed class RewardBundleCatalogConfigValidator : IDataValidator<RewardBundleEntry[]>
    {
        private const string BundlesCode = "reward-bundle.entries.required";
        private const string GrantModeCode = "reward-bundle.grant-mode.defined";
        private const string RewardsCode = "reward-bundle.rewards.not-empty";
        private const string RewardAmountCode = "reward.amount.positive";

        public void Validate(
            RewardBundleEntry[] bundles,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(bundles, context, BundlesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                bundles,
                bundle => bundle.Id,
                context);

            for (var bundleIndex = 0; bundleIndex < bundles.Length; bundleIndex++)
            {
                var bundle = bundles[bundleIndex];

                if (bundle == null)
                    continue;

                var bundleContext = context.At(bundleIndex);

                ValidationRules.DefinedEnum(
                    bundle.GrantMode,
                    bundleContext.At("GrantMode"),
                    GrantModeCode);

                var rewards = bundle.Rewards;
                var rewardsContext = bundleContext.At("Rewards");

                if (ValidationRules.NotEmpty(rewards, rewardsContext, RewardsCode) is false)
                    continue;

                IdentifierCollectionValidator.Validate(
                    rewards,
                    reward => reward.Id,
                    rewardsContext);

                for (var rewardIndex = 0; rewardIndex < rewards.Length; rewardIndex++)
                {
                    var reward = rewards[rewardIndex];

                    if (reward == null)
                        continue;

                    ValidationRules.Positive(
                        reward.Amount,
                        rewardsContext.At(rewardIndex).At("Amount"),
                        RewardAmountCode);
                }
            }
        }
    }
}