using LL.Game.Configuration;
using LL.Rewards.Configuration;
using LL.Rewards.Ranks;
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

        [MenuItem(MenuPath + "Card Catalog")]
        private static void SelectCardCatalog()
        {
            ConfigurationAssetMenu.Select<CardCatalogConfig>(CardCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Purchase Catalog")]
        private static void SelectPurchaseCatalog()
        {
            ConfigurationAssetMenu.Select<PurchaseCatalogConfig>(PurchaseCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Reward Bundle Catalog")]
        private static void SelectRewardBundleCatalog()
        {
            ConfigurationAssetMenu.Select<RewardBundleCatalogConfig>(RewardBundleCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Rank Reward Catalog")]
        private static void SelectRankRewardCatalog()
        {
            ConfigurationAssetMenu.Select<RankRewardCatalogConfig>(RankRewardCatalogConfig.CreationPath);
        }
    }
}