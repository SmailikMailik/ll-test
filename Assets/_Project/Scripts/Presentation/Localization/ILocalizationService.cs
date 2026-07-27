using System.Collections.Generic;

namespace LL.Presentation.Localization
{
    internal interface ILocalizationService
    {
        string GetText(string key);
        string GetText(string key, IReadOnlyDictionary<string, object> variables);
    }
}