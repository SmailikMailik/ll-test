using LL.Game.Cards.Configuration;
using LL.Game.Promotions.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using LLEditor.Configuration;
using LLEditor.Menu;
using UnityEditor;

namespace LLEditor.Game
{
    internal static class GameDataConfigMenu
    {
        [MenuItem(LLMenu.GamePath + "Ranks/Rank Catalog", false, LLMenu.ContentPriority)]
        private static void SelectRankCatalog()
        {
            ConfigurationAssetSelector.Select<RankCatalogConfig>(RankCatalogConfig.CreationPath);
        }

        [MenuItem(LLMenu.GamePath + "Promotions/Rank Promotion Catalog", false, LLMenu.ContentPriority)]
        private static void SelectRankPromotionCatalog()
        {
            ConfigurationAssetSelector.Select<RankPromotionCatalogConfig>(RankPromotionCatalogConfig.CreationPath);
        }

        [MenuItem(LLMenu.GamePath + "Cards/Card Catalog", false, LLMenu.ContentPriority)]
        private static void SelectCardCatalog()
        {
            ConfigurationAssetSelector.Select<CardCatalogConfig>(CardCatalogConfig.CreationPath);
        }

        [MenuItem(LLMenu.GamePath + "Rewards/Reward Catalog", false, LLMenu.ContentPriority)]
        private static void SelectRewardCatalog()
        {
            ConfigurationAssetSelector.Select<RewardCatalogConfig>(RewardCatalogConfig.CreationPath);
        }
    }
}