using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Heroes.Configuration
{
    internal sealed class HeroCatalogConfigValidator : IDataValidator<HeroEntry[]>
    {
        private const string EntriesCode = "hero-catalog.entries.not-empty";
        private const string NameKeyCode = "hero.name-localization-key.not-empty";

        public void Validate(HeroEntry[] heroes, ValidationContext context)
        {
            if (ValidationRules.NotEmpty(heroes, context, EntriesCode) is false)
                return;

            IdentifierCollectionValidator.Validate(heroes, hero => hero.Id, context);

            for (var index = 0; index < heroes.Length; index++)
            {
                var hero = heroes[index];

                if (hero is null)
                    continue;

                var heroContext = context.At(index);
                ValidationRules.NotEmpty(
                    hero.NameLocalizationKey,
                    heroContext.At(nameof(HeroEntry.NameLocalizationKey)),
                    NameKeyCode);
                IdentifierValidator.Validate(
                    hero.FlagId,
                    heroContext.At(nameof(HeroEntry.FlagId)));
            }
        }
    }
}