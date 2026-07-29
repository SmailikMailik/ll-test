using LL.Presentation.Configuration;
using LLEditor.Configuration;
using UnityEditor;

namespace LLEditor.Presentation
{
    internal static class PresentationConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "Presentation/";

        [MenuItem(MenuPath + "Item Icon Catalog")]
        private static void SelectItemIconCatalog()
        {
            ConfigurationAssetMenu.Select<ItemIconCatalogConfig>(ItemIconCatalogConfig.CreationPath);
        }
    }
}