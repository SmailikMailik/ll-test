using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Payments.Configuration
{
    internal sealed class PaymentEntryValidator : IDataValidator<PaymentEntry>
    {
        private const string EntryCode = "payment.entry.required";
        private const string AmountCode = "payment.amount.positive";

        public void Validate(PaymentEntry payment, ValidationContext context)
        {
            if (ValidationRules.NotNull(payment, context, EntryCode) is false)
                return;

            IdentifierValidator.Validate(
                payment.ItemId,
                context.At(nameof(PaymentEntry.ItemId)));
            ValidationRules.Positive(
                payment.Amount,
                context.At(nameof(PaymentEntry.Amount)),
                AmountCode);
        }
    }
}