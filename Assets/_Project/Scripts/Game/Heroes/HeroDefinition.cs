using System;
using LL.Game.Flags;
using LL.Game.Identifiers;
using LL.Validation;

namespace LL.Game.Heroes
{
    internal sealed class HeroDefinition
    {
        internal HeroId Id { get; }
        internal string NameLocalizationKey { get; }
        internal FlagId FlagId { get; }

        internal HeroDefinition(
            HeroId id,
            string nameLocalizationKey,
            FlagId flagId)
        {
            IdentifierValidator.EnsureValid(id, nameof(id));

            if (ValidationChecks.IsEmpty(nameLocalizationKey))
                throw new ArgumentException("Hero name localization key must be non-empty.", nameof(nameLocalizationKey));

            IdentifierValidator.EnsureValid(flagId, nameof(flagId));

            Id = id;
            NameLocalizationKey = nameLocalizationKey;
            FlagId = flagId;
        }
    }
}