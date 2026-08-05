using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Presentation.Flags.Configuration
{
    internal sealed class FlagCatalogConfigValidator : IDataValidator<FlagEntry[]>
    {
        private const string EntriesCode = "flag.entries.not-empty";
        private const string FlagCode = "flag.sprite.required";

        public void Validate(FlagEntry[] flags, ValidationContext context)
        {
            if (ValidationRules.NotEmpty(flags, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(flags, entry => entry.Id, context);

            for (var index = 0; index < flags.Length; index++)
            {
                var entry = flags[index];

                if (entry is null)
                    continue;

                ValidationRules.NotNull(
                    entry.Flag,
                    context.At(index).At(nameof(FlagEntry.Flag)),
                    FlagCode);
            }
        }
    }
}