using LL.Presentation.Icons.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class PresentationConfigMenu
    {
        [MenuItem(LastLevelMenu.ContentPath + "Item Icon Catalog", false, LastLevelMenu.ProjectContentPriority)]
        private static void SelectItemIconCatalog()
        {
            ProjectAssetSelector.Select<ItemIconCatalogConfig>(ItemIconCatalogConfig.CreationPath);
        }
    }
}