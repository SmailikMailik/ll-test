using System;
using LL.Game.Countries;
using LL.Game.Identifiers;

namespace LL.Game.Heroes
{
    internal sealed class HeroDefinition
    {
        internal HeroId Id { get; }
        internal string NameLocalizationKey { get; }
        internal CountryId CountryId { get; }

        internal HeroDefinition(
            HeroId id,
            string nameLocalizationKey,
            CountryId countryId)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));

            if (string.IsNullOrWhiteSpace(nameLocalizationKey))
                throw new ArgumentException("Hero name localization key must be non-empty.", nameof(nameLocalizationKey));

            CountryIdValidator.EnsureValid(countryId, nameof(countryId));

            Id = id;
            NameLocalizationKey = nameLocalizationKey;
            CountryId = countryId;
        }
    }
}