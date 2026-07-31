using LL.User.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class UserConfigMenu
    {
        [MenuItem(LastLevelMenu.Content.Path + "User Defaults", false, LastLevelMenu.Content.UserPriority)]
        private static void SelectUserDefaultsConfig()
        {
            ProjectAssetSelector.Select<UserDefaultsConfig>(UserDefaultsConfig.CreationPath);
        }
    }
}