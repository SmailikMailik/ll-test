using LL.User.Snapshots;

namespace LL.User.Configuration
{
    internal interface IUserDefaultsFactory
    {
        UserSnapshot CreateSnapshot();
    }
}