using LL.User.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class UserConfigMenu
    {
        [MenuItem(LastLevelMenu.ContentPath + "User Defaults", false, LastLevelMenu.UserContentPriority)]
        private static void SelectUserDefaultsConfig()
        {
            ProjectAssetSelector.Select<UserDefaultsConfig>(UserDefaultsConfig.CreationPath);
        }
    }
}