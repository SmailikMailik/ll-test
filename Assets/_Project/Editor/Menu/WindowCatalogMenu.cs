using LL.UI.Windows.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class WindowCatalogMenu
    {
        [MenuItem(LastLevelMenu.Content.Path + "Window Catalog", false, LastLevelMenu.Content.UIPriority)]
        private static void SelectWindowCatalog()
        {
            ProjectAssetSelector.Select<WindowCatalogConfig>(WindowCatalogConfig.CreationPath);
        }
    }
}