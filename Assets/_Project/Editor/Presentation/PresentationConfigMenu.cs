using LL.Presentation.Configuration;
using LLEditor.Configuration;
using UnityEditor;

namespace LLEditor.Presentation
{
    internal static class PresentationConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "Presentation/";

        [MenuItem(MenuPath + "Currency Icon Catalog")]
        private static void SelectCurrencyIconCatalog()
        {
            ConfigurationAssetMenu.Select<CurrencyIconCatalogConfig>(CurrencyIconCatalogConfig.CreationPath);
        }

        [MenuItem(MenuPath + "Card Icon Catalog")]
        private static void SelectCardIconCatalog()
        {
            ConfigurationAssetMenu.Select<CardIconCatalogConfig>(CardIconCatalogConfig
                .CreationPath);
        }
    }
}