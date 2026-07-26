using LL.User.Configuration;
using UnityEditor;

namespace LLEditor
{
    internal static class UserConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "User/";

        [MenuItem(MenuPath + "User Defaults Config")]
        private static void SelectUserDefaultsConfig()
        {
            ConfigurationAssetMenu.Select<UserDefaultsConfig>(UserDefaultsConfig.CreationPath);
        }
    }
}