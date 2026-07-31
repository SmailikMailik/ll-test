using LL.Game.Countries;
using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Presentation.Countries.Configuration
{
    internal sealed class CountryFlagCatalogConfigValidator : IDataValidator<CountryFlagEntry[]>
    {
        private const string EntriesCode = "country-flag.entries.not-empty";
        private const string FlagCode = "country-flag.sprite.required";

        public void Validate(CountryFlagEntry[] flags, ValidationContext context)
        {
            if (ValidationRules.NotEmpty(flags, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(flags, entry => entry.CountryId, context);

            for (var index = 0; index < flags.Length; index++)
            {
                var entry = flags[index];

                if (entry == null)
                    continue;

                var entryContext = context.At(index);
                CountryIdValidator.Validate(
                    entry.CountryId,
                    entryContext.At(nameof(CountryFlagEntry.CountryId)));
                ValidationRules.NotNull(
                    entry.Flag,
                    entryContext.At(nameof(CountryFlagEntry.Flag)),
                    FlagCode);
            }
        }
    }
}