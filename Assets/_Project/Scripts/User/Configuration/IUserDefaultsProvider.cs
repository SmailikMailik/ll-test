using LL.User.Core;

namespace LL.User.Configuration
{
    internal interface IUserDefaultsProvider
    {
        UserInitialData GetDefaults();
    }
}