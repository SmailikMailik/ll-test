using LL.Game.Cards.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class GameDataConfigMenu
    {
        [MenuItem(LastLevelMenu.GamePath + "Ranks/Rank Catalog", false, LastLevelMenu.ContentPriority)]
        private static void SelectRankCatalog()
        {
            ProjectAssetSelector.Select<RankCatalogConfig>(RankCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.GamePath + "Promotions/Rank Promotion Catalog", false, LastLevelMenu.ContentPriority)]
        private static void SelectRankPromotionCatalog()
        {
            ProjectAssetSelector.Select<RankPromotionCatalogConfig>(RankPromotionCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.GamePath + "Cards/Card Catalog", false, LastLevelMenu.ContentPriority)]
        private static void SelectCardCatalog()
        {
            ProjectAssetSelector.Select<CardCatalogConfig>(CardCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.GamePath + "Rewards/Reward Catalog", false, LastLevelMenu.ContentPriority)]
        private static void SelectRewardCatalog()
        {
            ProjectAssetSelector.Select<RewardCatalogConfig>(RewardCatalogConfig.CreationPath);
        }
    }
}