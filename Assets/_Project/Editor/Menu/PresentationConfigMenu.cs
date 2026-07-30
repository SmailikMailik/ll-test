using LL.Presentation.Icons.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class PresentationConfigMenu
    {
        [MenuItem(LastLevelMenu.PresentationPath + "Icons/Item Icon Catalog", false, LastLevelMenu.ContentPriority)]
        private static void SelectItemIconCatalog()
        {
            ProjectAssetSelector.Select<ItemIconCatalogConfig>(ItemIconCatalogConfig.CreationPath);
        }
    }
}