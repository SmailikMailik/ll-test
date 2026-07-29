using LL.Game.Cards.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LLEditor.Configuration;
using UnityEditor;

namespace LLEditor.Game
{
    internal static class GameDataConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "Game Data/";

        [MenuItem(MenuPath + "Rank Catalog")]
        private static void SelectRankCatalog()
        {
            ConfigurationAssetMenu.Select<RankCatalogConfig>(RankCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Rank Promotion Catalog")]
        private static void SelectRankPromotionCatalog()
        {
            ConfigurationAssetMenu.Select<RankPromotionCatalogConfig>(RankPromotionCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Card Catalog")]
        private static void SelectCardCatalog()
        {
            ConfigurationAssetMenu.Select<CardCatalogConfig>(CardCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Reward Bundle Catalog")]
        private static void SelectRewardBundleCatalog()
        {
            ConfigurationAssetMenu.Select<RewardBundleCatalogConfig>(RewardBundleCatalogConfig.CreationPath);
        }
    }
}