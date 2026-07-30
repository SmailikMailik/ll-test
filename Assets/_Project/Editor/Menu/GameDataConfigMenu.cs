using LL.Game.Cards.Configuration;
using LL.Game.RankUp.Configuration;
using LL.Game.Quests.Configuration;
using LL.Game.Ranks.Configuration;
using LL.Game.Rewards.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class GameDataConfigMenu
    {
        [MenuItem(LastLevelMenu.ContentPath + "Rank Catalog", false, LastLevelMenu.GameContentPriority + 2)]
        private static void SelectRankCatalog()
        {
            ProjectAssetSelector.Select<RankCatalogConfig>(RankCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.ContentPath + "Rank-Up Catalog", false, LastLevelMenu.GameContentPriority + 3)]
        private static void SelectRankUpCatalog()
        {
            ProjectAssetSelector.Select<RankUpCatalogConfig>(RankUpCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.ContentPath + "Quest Catalog", false, LastLevelMenu.GameContentPriority + 1)]
        private static void SelectQuestCatalog()
        {
            ProjectAssetSelector.Select<QuestCatalogConfig>(QuestCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.ContentPath + "Card Catalog", false, LastLevelMenu.GameContentPriority)]
        private static void SelectCardCatalog()
        {
            ProjectAssetSelector.Select<CardCatalogConfig>(CardCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.ContentPath + "Reward Catalog", false, LastLevelMenu.GameContentPriority + 4)]
        private static void SelectRewardCatalog()
        {
            ProjectAssetSelector.Select<RewardCatalogConfig>(RewardCatalogConfig.CreationPath);
        }
    }
}