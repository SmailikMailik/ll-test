using LL.User.Configuration;
using LLEditor.Configuration;
using UnityEditor;

namespace LLEditor.User
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