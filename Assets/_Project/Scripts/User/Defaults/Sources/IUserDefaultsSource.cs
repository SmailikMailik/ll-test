using LL.User.Defaults.Declarations;

namespace LL.User.Defaults.Sources
{
    internal interface IUserDefaultsSource
    {
        UserDefaultsDeclaration Read();
    }
}