using LL.UI.Windows.Configuration;
using LLEditor.Configuration;
using UnityEditor;

namespace LLEditor.UI.Windows
{
    internal static class WindowCatalogMenu
    {
        [MenuItem(ConfigurationAssetMenu.RootPath + "Window Catalog")]
        private static void SelectWindowCatalog()
        {
            ConfigurationAssetMenu.Select<WindowCatalog>(WindowCatalog.CreationPath);
        }
    }
}