using LL.Game.Cards.Configuration;
using LL.Game.Data.Configuration;
using LL.Game.Heroes.Configuration;
using LL.Game.Quests.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.RankUp.Configuration;
using LL.Game.Rewards.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class GameDataConfigMenu
    {
        [MenuItem(LastLevelMenu.Content.Path + "Game Data Manifest", false, LastLevelMenu.Content.GamePriority)]
        private static void SelectGameDataManifest()
        {
            ProjectAssetSelector.Select<GameDataManifestConfig>(GameDataManifestConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Card Catalog", false, LastLevelMenu.Content.GamePriority + 1)]
        private static void SelectCardCatalog()
        {
            ProjectAssetSelector.Select<CardCatalogConfig>(CardCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Hero Catalog", false, LastLevelMenu.Content.GamePriority + 2)]
        private static void SelectHeroCatalog()
        {
            ProjectAssetSelector.Select<HeroCatalogConfig>(HeroCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Quest Catalog", false, LastLevelMenu.Content.GamePriority + 3)]
        private static void SelectQuestCatalog()
        {
            ProjectAssetSelector.Select<QuestCatalogConfig>(QuestCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Rank Catalog", false, LastLevelMenu.Content.GamePriority + 4)]
        private static void SelectRankCatalog()
        {
            ProjectAssetSelector.Select<RankCatalogConfig>(RankCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Rank-Up Catalog", false, LastLevelMenu.Content.GamePriority + 5)]
        private static void SelectRankUpCatalog()
        {
            ProjectAssetSelector.Select<RankUpCatalogConfig>(RankUpCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Reward Catalog", false, LastLevelMenu.Content.GamePriority + 6)]
        private static void SelectRewardCatalog()
        {
            ProjectAssetSelector.Select<RewardCatalogConfig>(RewardCatalogConfig.CreationPath);
        }
    }
}