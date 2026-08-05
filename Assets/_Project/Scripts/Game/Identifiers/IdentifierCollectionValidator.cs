using System;
using System.Collections.Generic;
using LL.Validation;

namespace LL.Game.Identifiers
{
    internal static class IdentifierCollectionValidator
    {
        private const string MissingEntryCode = "identifier.entry.required";
        private const string DuplicateIdentifierCode = "identifier.duplicate";

        internal static ValidationResult Validate<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId)
            where TId : struct, IIdentifier =>
            ValidationRunner.Run(context => Validate(entries, getId, context));

        internal static void Validate<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            ValidationContext context)
            where TId : struct, IIdentifier
        {
            if (getId is null)
                throw new ArgumentNullException(nameof(getId));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var usedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var index = 0;

            if (entries is null)
                return;

            foreach (var entry in entries)
            {
                var entryContext = context.At(index);

                if (ValidationRules.NotNull(entry, entryContext, MissingEntryCode) is false)
                {
                    index++;
                    continue;
                }

                var id = getId(entry);
                var idContext = entryContext.At("Id");
                IdentifierValidator.Validate(id, idContext);

                if (ValidationChecks.IsNotEmpty(id.Value))
                {
                    var comparisonValue = id.Value.Trim();

                    ValidationRules.TryAddUnique(
                        comparisonValue,
                        usedValues,
                        idContext,
                        DuplicateIdentifierCode);
                }

                index++;
            }
        }

        internal static bool IsValid<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId)
            where TId : struct, IIdentifier =>
            ValidationRunner.IsValid(context => Validate(entries, getId, context));

        internal static void EnsureValid<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            string parameterName)
            where TId : struct, IIdentifier =>
            ValidationRunner.EnsureValid(context => Validate(entries, getId, context), parameterName);
    }
}