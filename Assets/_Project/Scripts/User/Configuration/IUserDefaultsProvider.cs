using LL.User.Snapshots;

namespace LL.User.Configuration
{
    internal interface IUserDefaultsProvider
    {
        UserSnapshot GetDefaultSnapshot();
    }
}