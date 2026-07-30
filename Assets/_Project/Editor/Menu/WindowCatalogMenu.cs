using LL.UI.Windows.Configuration;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class WindowCatalogMenu
    {
        [MenuItem(LastLevelMenu.UIPath + "Windows/Window Catalog", false, LastLevelMenu.ContentPriority)]
        private static void SelectWindowCatalog()
        {
            ProjectAssetSelector.Select<WindowCatalog>(WindowCatalog.CreationPath);
        }
    }
}