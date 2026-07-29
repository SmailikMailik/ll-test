using LL.Game.Data.Validation;
using LL.Game.Identifiers;

namespace LL.User.Configuration
{
    internal sealed class UserItemsDefaultsValidator : IDataValidator<ItemAmountEntry[]>
    {
        private const string AmountCode = "user-defaults.item.amount.non-negative";

        public void Validate(
            ItemAmountEntry[] items,
            ValidationContext context)
        {
            IdentifierCollectionValidator.Validate(
                items,
                item => item.Id,
                context);

            if (items == null)
                return;

            for (var index = 0; index < items.Length; index++)
            {
                var item = items[index];

                if (item == null)
                    continue;

                ValidationRules.NonNegative(
                    item.Amount,
                    context.At(index).At("Amount"),
                    AmountCode);
            }
        }
    }
}