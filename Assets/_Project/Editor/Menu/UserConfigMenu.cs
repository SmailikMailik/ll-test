using LL.User.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class UserConfigMenu
    {
        [MenuItem(LastLevelMenu.UserPath + "Configuration/User Defaults", false, LastLevelMenu.ContentPriority)]
        private static void SelectUserDefaultsConfig()
        {
            ProjectAssetSelector.Select<UserDefaultsConfig>(UserDefaultsConfig.CreationPath);
        }
    }
}