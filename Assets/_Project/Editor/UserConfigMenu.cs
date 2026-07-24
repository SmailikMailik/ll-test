using LL.User;
using UnityEditor;

namespace LLEditor
{
    internal static class UserConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "User/";

        [MenuItem(MenuPath + "User Data Config")]
        private static void SelectUserDataConfig()
        {
            ConfigurationAssetMenu.Select<UserDataConfig>(UserDataConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Level Progression Config")]
        private static void SelectLevelProgressionConfig()
        {
            ConfigurationAssetMenu.Select<LevelProgressionConfig>(LevelProgressionConfig.CreationPath);
        }
    }
}