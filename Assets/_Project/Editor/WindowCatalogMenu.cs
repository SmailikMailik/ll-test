using LL.UI.Windows.Configuration;
using UnityEditor;

namespace LLEditor
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