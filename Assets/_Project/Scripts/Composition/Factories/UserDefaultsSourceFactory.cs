using LL.Infrastructure.Loading;
using LL.User.Configuration;
using LL.User.Defaults.Declarations;
using LL.User.Defaults.Sources;

namespace LL.Composition.Factories
{
    internal static class UserDefaultsSourceFactory
    {
        internal static IDataSource<UserDefaultsDeclaration> CreateFromScriptableObject(UserDefaultsConfig config)
        {
            return new ScriptableObjectUserDefaultsSource(config);
        }
    }
}