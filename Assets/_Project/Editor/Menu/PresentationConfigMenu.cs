using LL.Presentation.Flags.Configuration;
using LL.Presentation.Heroes.Configuration;
using LL.Presentation.Items.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class PresentationConfigMenu
    {
        [MenuItem(LastLevelMenu.Content.Path + "Item Icon Catalog", false, LastLevelMenu.Content.PresentationPriority)]
        private static void SelectItemIconCatalog()
        {
            ProjectAssetSelector.Select<ItemIconCatalogConfig>(ItemIconCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Flag Catalog", false, LastLevelMenu.Content.PresentationPriority + 1)]
        private static void SelectFlagCatalog()
        {
            ProjectAssetSelector.Select<FlagCatalogConfig>(FlagCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Hero Portrait Catalog", false, LastLevelMenu.Content.PresentationPriority + 2)]
        private static void SelectHeroPortraitCatalog()
        {
            ProjectAssetSelector.Select<HeroPortraitCatalogConfig>(HeroPortraitCatalogConfig.CreationPath);
        }
    }
}