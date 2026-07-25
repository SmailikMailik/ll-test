using LL.Game.Configuration;
using UnityEditor;

namespace LLEditor
{
    internal static class GameDataConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "Game Data/";

        [MenuItem(MenuPath + "Rank Progression Config")]
        private static void SelectRankProgressionConfig()
        {
            ConfigurationAssetMenu.Select<RankProgressionConfig>(
                RankProgressionConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Experience Card Catalog")]
        private static void SelectExperienceCardCatalog()
        {
            ConfigurationAssetMenu.Select<ExperienceCardCatalogConfig>(
                ExperienceCardCatalogConfig.CreationPath);
        }
    }
}