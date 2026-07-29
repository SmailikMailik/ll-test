using LL.User.Core;

namespace LL.User.Configuration
{
    internal interface IUserDefaultsProvider
    {
        UserData GetDefaults();
    }
}