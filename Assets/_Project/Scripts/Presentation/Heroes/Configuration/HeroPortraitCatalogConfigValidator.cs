using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Presentation.Heroes.Configuration
{
    internal sealed class HeroPortraitCatalogConfigValidator : IDataValidator<HeroPortraitEntry[]>
    {
        private const string EntriesCode = "hero-portrait.entries.not-empty";
        private const string SmallPortraitCode = "hero-portrait.small.required";
        private const string LargePortraitCode = "hero-portrait.large.required";

        public void Validate(HeroPortraitEntry[] portraits, ValidationContext context)
        {
            if (ValidationRules.NotEmpty(portraits, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(portraits, entry => entry.HeroId, context);

            for (var index = 0; index < portraits.Length; index++)
            {
                var entry = portraits[index];

                if (entry is null)
                    continue;

                var entryContext = context.At(index);
                ValidationRules.NotNull(
                    entry.SmallPortrait,
                    entryContext.At(nameof(HeroPortraitEntry.SmallPortrait)),
                    SmallPortraitCode);
                ValidationRules.NotNull(
                    entry.LargePortrait,
                    entryContext.At(nameof(HeroPortraitEntry.LargePortrait)),
                    LargePortraitCode);
            }
        }
    }
}