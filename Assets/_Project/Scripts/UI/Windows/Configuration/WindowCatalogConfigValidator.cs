using System;
using System.Collections.Generic;
using LL.Validation;

namespace LL.UI.Windows.Configuration
{
    internal sealed class WindowCatalogConfigValidator : IDataValidator<IReadOnlyList<WindowEntry>>
    {
        private const string EntriesCode = "window.entries.required";
        private const string EntryCode = "window.entry.required";
        private const string PrefabCode = "window.prefab.required";
        private const string ParameterTypeCode = "window.parameters.unique";

        public void Validate(
            IReadOnlyList<WindowEntry> entries,
            ValidationContext context)
        {
            if (ValidationRules.NotNull(entries, context, EntriesCode) is false)
                return;

            var parameterTypes = new HashSet<Type>();

            for (var index = 0; index < entries.Count; index++)
            {
                var entry = entries[index];
                var entryContext = context.At(index);

                if (ValidationRules.NotNull(entry, entryContext, EntryCode) is false)
                    continue;

                if (ValidationRules.NotNull(
                        entry.Prefab,
                        entryContext.At(nameof(WindowEntry.Prefab)),
                        PrefabCode) is false)
                {
                    continue;
                }

                var parameterType = entry.Prefab.ParameterType;
                ValidationRules.TryAddUnique(
                    parameterType,
                    parameterTypes,
                    entryContext.At(nameof(WindowEntry.Prefab)),
                    ParameterTypeCode);
            }
        }
    }
}