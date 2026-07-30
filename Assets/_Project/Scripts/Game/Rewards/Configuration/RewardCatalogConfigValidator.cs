using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Rewards.Configuration
{
    internal sealed class RewardCatalogConfigValidator : IDataValidator<RewardEntry[]>
    {
        private const string EntriesCode = "reward.entries.required";
        private const string ItemsCode = "reward.items.not-empty";
        private const string ItemAmountCode = "reward.item.amount.positive";

        public void Validate(
            RewardEntry[] rewards,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(rewards, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                rewards,
                reward => reward.Id,
                context);

            for (var rewardIndex = 0; rewardIndex < rewards.Length; rewardIndex++)
            {
                var reward = rewards[rewardIndex];

                if (reward == null)
                    continue;

                var rewardContext = context.At(rewardIndex);

                var items = reward.Items;
                var itemsContext = rewardContext.At(nameof(RewardEntry.Items));

                if (ValidationRules.NotEmpty(items, itemsContext, ItemsCode) is false)
                    continue;

                IdentifierCollectionValidator.Validate(
                    items,
                    item => item.Id,
                    itemsContext);

                for (var itemIndex = 0; itemIndex < items.Count; itemIndex++)
                {
                    var item = items[itemIndex];

                    if (item == null)
                        continue;

                    ValidationRules.Positive(
                        item.Amount,
                        itemsContext.At(itemIndex).At(nameof(RewardItemEntry.Amount)),
                        ItemAmountCode);
                }
            }
        }
    }
}