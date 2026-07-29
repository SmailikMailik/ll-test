using LL.Game.Identifiers;
using LL.Validation;

namespace LL.User.Configuration
{
    internal sealed class UserItemsDefaultsValidator : IDataValidator<ItemAmountEntry[]>
    {
        private const string ItemsCode = "user-defaults.items.required";
        private const string AmountCode = "user-defaults.item.amount.non-negative";

        public void Validate(
            ItemAmountEntry[] items,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(items, context, ItemsCode) is false)
                return;

            IdentifierCollectionValidator.Validate(
                items,
                item => item.Id,
                context);

            for (var index = 0; index < items.Length; index++)
            {
                var item = items[index];

                if (item == null)
                    continue;

                ValidationRules.NonNegative(
                    item.Amount,
                    context.At(index).At(nameof(ItemAmountEntry.Amount)),
                    AmountCode);
            }
        }
    }
}