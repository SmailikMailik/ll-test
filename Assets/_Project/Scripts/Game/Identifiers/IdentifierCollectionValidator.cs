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
            where TId : struct, IIdentifier
        {
            var result = new ValidationResult();
            Validate(entries, getId, new ValidationContext(result));
            return result;
        }

        internal static void Validate<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            ValidationContext context)
            where TId : struct, IIdentifier
        {
            if (getId == null)
                throw new ArgumentNullException(nameof(getId));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var usedValues = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var index = 0;

            if (entries == null)
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

                if (string.IsNullOrWhiteSpace(id.Value) is false)
                {
                    var comparisonValue = id.Value.Trim();

                    ValidationRules.Unique(
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
            where TId : struct, IIdentifier
        {
            return Validate(entries, getId).IsValid;
        }

        internal static void EnsureValid<TEntry, TId>(
            IEnumerable<TEntry> entries,
            Func<TEntry, TId> getId,
            string parameterName)
            where TId : struct, IIdentifier
        {
            ValidationResultGuard.EnsureValid(
                Validate(entries, getId),
                parameterName);
        }
    }
}