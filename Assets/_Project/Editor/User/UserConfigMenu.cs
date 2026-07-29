using LL.User.Configuration;
using LLEditor.Configuration;
using LLEditor.Menu;
using UnityEditor;

namespace LLEditor.User
{
    internal static class UserConfigMenu
    {
        [MenuItem(LLMenu.UserPath + "Configuration/User Defaults", false, LLMenu.ContentPriority)]
        private static void SelectUserDefaultsConfig()
        {
            ConfigurationAssetSelector.Select<UserDefaultsConfig>(UserDefaultsConfig.CreationPath);
        }
    }
}