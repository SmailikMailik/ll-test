using LL.Presentation.Icons.Configuration;
using LLEditor.Configuration;
using LLEditor.Menu;
using UnityEditor;

namespace LLEditor.Presentation
{
    internal static class PresentationConfigMenu
    {
        [MenuItem(LLMenu.PresentationPath + "Icons/Item Icon Catalog", false, LLMenu.ContentPriority)]
        private static void SelectItemIconCatalog()
        {
            ConfigurationAssetSelector.Select<ItemIconCatalogConfig>(ItemIconCatalogConfig.CreationPath);
        }
    }
}