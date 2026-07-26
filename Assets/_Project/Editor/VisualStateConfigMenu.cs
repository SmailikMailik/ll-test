using LL.UI.VisualStates;
using UnityEditor;

namespace LLEditor
{
    internal static class VisualStateConfigMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "UI/";

        [MenuItem(MenuPath + "Visual State Sets")]
        private static void SelectVisualStateSets()
        {
            ConfigurationAssetMenu.SelectAll<VisualStateSet>(VisualStateSet.CreationPath);
        }
    }
}