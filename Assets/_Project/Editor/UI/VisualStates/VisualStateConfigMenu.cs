using LL.UI.VisualStates.Core;
using LLEditor.Configuration;
using UnityEditor;

namespace LLEditor.UI.VisualStates
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