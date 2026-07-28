using System.Collections.Generic;
using R3;

namespace LL.Presentation.Localization
{
    internal interface ILocalizationService
    {
        Observable<Unit> LocaleChanged { get; }

        string GetText(string key);
        string GetText(string key, IReadOnlyDictionary<string, object> variables);
    }
}