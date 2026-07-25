using LL.User.Configuration;
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

        [MenuItem(MenuPath + "Rank Progression Config")]
        private static void SelectRankProgressionConfig()
        {
            ConfigurationAssetMenu.Select<RankProgressionConfig>(RankProgressionConfig.CreationPath);
        }
    }
}