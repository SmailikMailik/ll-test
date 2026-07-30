using LL.UI.Windows.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class WindowCatalogMenu
    {
        [MenuItem(LastLevelMenu.ContentPath + "Window Catalog", false, LastLevelMenu.ProjectContentPriority + 1)]
        private static void SelectWindowCatalog()
        {
            ProjectAssetSelector.Select<WindowCatalogConfig>(WindowCatalogConfig.CreationPath);
        }
    }
}