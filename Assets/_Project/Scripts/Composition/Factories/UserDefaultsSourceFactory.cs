using LL.User.Configuration;
using LL.User.Defaults.Sources;

namespace LL.Composition.Factories
{
    internal static class UserDefaultsSourceFactory
    {
        internal static IUserDefaultsSource CreateFromScriptableObject(UserDefaultsConfig config)
        {
            return new ScriptableObjectUserDefaultsSource(config);
        }
    }
}