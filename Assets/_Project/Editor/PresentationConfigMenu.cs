using LL.Presentation.Configuration;
using UnityEditor;

namespace LLEditor
{
    internal static class PresentationConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "Presentation/";

        [MenuItem(MenuPath + "Currency Icon Catalog")]
        private static void SelectCurrencyIconCatalog()
        {
            ConfigurationAssetMenu.Select<CurrencyIconCatalogConfig>(
                CurrencyIconCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Experience Card Icon Catalog")]
        private static void SelectExperienceCardIconCatalog()
        {
            ConfigurationAssetMenu.Select<ExperienceCardIconCatalogConfig>(
                ExperienceCardIconCatalogConfig.CreationPath);
        }
    }
}