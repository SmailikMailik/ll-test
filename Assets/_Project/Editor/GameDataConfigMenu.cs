using LL.Game.Configuration;
using UnityEditor;

namespace LLEditor
{
    internal static class GameDataConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "Game Data/";

        [MenuItem(MenuPath + "Rank Catalog")]
        private static void SelectRankCatalog()
        {
            ConfigurationAssetMenu.Select<RankCatalogConfig>(RankCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Experience Card Catalog")]
        private static void SelectExperienceCardCatalog()
        {
            ConfigurationAssetMenu.Select<ExperienceCardCatalogConfig>(ExperienceCardCatalogConfig.CreationPath);
        }
    }
}