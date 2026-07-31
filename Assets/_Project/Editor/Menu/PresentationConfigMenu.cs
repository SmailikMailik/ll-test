using LL.Presentation.Countries.Configuration;
using LL.Presentation.Heroes.Configuration;
using LL.Presentation.Icons.Configuration;
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

        [MenuItem(LastLevelMenu.Content.Path + "Country Flag Catalog", false, LastLevelMenu.Content.PresentationPriority + 1)]
        private static void SelectCountryFlagCatalog()
        {
            ProjectAssetSelector.Select<CountryFlagCatalogConfig>(CountryFlagCatalogConfig.CreationPath);
        }

        [MenuItem(LastLevelMenu.Content.Path + "Hero Portrait Catalog", false, LastLevelMenu.Content.PresentationPriority + 2)]
        private static void SelectHeroPortraitCatalog()
        {
            ProjectAssetSelector.Select<HeroPortraitCatalogConfig>(HeroPortraitCatalogConfig.CreationPath);
        }
    }
}