using LL.UI.Windows.Configuration;
using LLEditor.Configuration;
using LLEditor.Menu;
using UnityEditor;

namespace LLEditor.UI.Windows
{
    internal static class WindowCatalogMenu
    {
        [MenuItem(LLMenu.UIPath + "Windows/Window Catalog", false, LLMenu.ContentPriority)]
        private static void SelectWindowCatalog()
        {
            ConfigurationAssetSelector.Select<WindowCatalog>(WindowCatalog.CreationPath);
        }
    }
}