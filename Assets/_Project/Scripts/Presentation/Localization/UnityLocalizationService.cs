using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using VContainer;

namespace LL.Presentation.Localization
{
    internal sealed class UnityLocalizationService : ILocalizationService
    {
        private const char ScopeSeparator = '/';

        [Inject]
        internal UnityLocalizationService() { }

        public string GetText(string key) => GetText(key, null);

        public string GetText(string key, IReadOnlyDictionary<string, object> variables)
        {
            GetUnityKey(key, out var table, out var entry);

            if (variables == null || variables.Count == 0)
                return LocalizationSettings.StringDatabase.GetLocalizedString(table, entry);

            return LocalizationSettings.StringDatabase.GetLocalizedString(
                table,
                entry,
                new object[] { CreateSmartVariables(variables) });
        }

        private static Dictionary<string, object> CreateSmartVariables(IReadOnlyDictionary<string, object> variables)
        {
            var smartVariables = new Dictionary<string, object>(variables.Count);

            foreach (var variable in variables)
                smartVariables.Add(variable.Key, variable.Value);

            return smartVariables;
        }

        private static void GetUnityKey(string key, out string table, out string entry)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Localization key cannot be empty.", nameof(key));

            var separatorIndex = key.IndexOf(ScopeSeparator);

            if (separatorIndex <= 0 || separatorIndex >= key.Length - 1)
                throw new ArgumentException($"Invalid localization key: '{key}'.", nameof(key));

            table = key.Substring(0, separatorIndex);
            entry = key.Substring(separatorIndex + 1);
        }
    }
}