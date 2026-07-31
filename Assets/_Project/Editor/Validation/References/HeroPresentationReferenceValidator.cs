using System.Collections.Generic;
using LL.Game.Countries;
using LL.Game.Heroes;
using LL.Game.Heroes.Configuration;
using LL.Game.Identifiers;
using LL.Presentation.Countries.Configuration;
using LL.Presentation.Heroes.Configuration;
using LL.Validation;
using LLEditor.Validation.Sources;
using UnityEditor;

namespace LLEditor.Validation.References
{
    internal sealed class HeroPresentationReferenceValidator : IProjectDataReferenceValidation
    {
        private const string CountryFlagExistsCode = "hero.country-flag.exists";
        private const string HeroPortraitExistsCode = "hero.portrait.exists";
        private const string PortraitHeroExistsCode = "hero-portrait.hero.exists";

        public void Validate(ProjectDataSources sources, ValidationContext context)
        {
            var heroes = sources.GetSingle<HeroCatalogConfig>();
            var flags = sources.GetSingle<CountryFlagCatalogConfig>();
            var portraits = sources.GetSingle<HeroPortraitCatalogConfig>();

            if (heroes == null || flags == null || portraits == null)
                return;

            var countryIds = CollectCountryIds(flags);
            var portraitHeroIds = CollectPortraitHeroIds(portraits);
            var heroIds = CollectHeroIds(heroes);
            var heroContext = context.At(AssetDatabase.GetAssetPath(heroes));

            for (var index = 0; index < heroes.Heroes.Count; index++)
            {
                var hero = heroes.Heroes[index];

                if (hero is null || IdentifierValidator.IsValid(hero.Id) is false)
                    continue;

                ValidationRules.ReferenceExists(
                    hero.CountryId,
                    countryIds,
                    heroContext.At(index).At(nameof(HeroEntry.CountryId)),
                    CountryFlagExistsCode);
                ValidationRules.ReferenceExists(
                    hero.Id,
                    portraitHeroIds,
                    heroContext.At(index).At(nameof(HeroEntry.Id)),
                    HeroPortraitExistsCode);
            }

            var portraitContext = context.At(AssetDatabase.GetAssetPath(portraits));

            for (var index = 0; index < portraits.Portraits.Count; index++)
            {
                var portrait = portraits.Portraits[index];

                if (portrait is null || IdentifierValidator.IsValid(portrait.HeroId) is false)
                    continue;

                ValidationRules.ReferenceExists(
                    portrait.HeroId,
                    heroIds,
                    portraitContext.At(index).At(nameof(HeroPortraitEntry.HeroId)),
                    PortraitHeroExistsCode);
            }
        }

        private static HashSet<CountryId> CollectCountryIds(CountryFlagCatalogConfig flags)
        {
            var ids = new HashSet<CountryId>();

            foreach (var flag in flags.Flags)
            {
                if (flag is not null && CountryIdValidator.IsValid(flag.CountryId))
                    ids.Add(flag.CountryId);
            }

            return ids;
        }

        private static HashSet<HeroId> CollectPortraitHeroIds(HeroPortraitCatalogConfig portraits)
        {
            var ids = new HashSet<HeroId>();

            foreach (var portrait in portraits.Portraits)
            {
                if (portrait is not null && IdentifierValidator.IsValid(portrait.HeroId))
                    ids.Add(portrait.HeroId);
            }

            return ids;
        }

        private static HashSet<HeroId> CollectHeroIds(HeroCatalogConfig heroes)
        {
            var ids = new HashSet<HeroId>();

            foreach (var hero in heroes.Heroes)
            {
                if (hero is not null && IdentifierValidator.IsValid(hero.Id))
                    ids.Add(hero.Id);
            }

            return ids;
        }
    }
}