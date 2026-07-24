using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LL.UI.Windows
{
    internal static class WindowCatalogMenu
    {
        internal const string RootPath = "Tools/";
        internal const string CreationPath = "LL/Window Catalog";

#if UNITY_EDITOR
        [MenuItem(RootPath + CreationPath)]
        private static void SelectObject()
        {
            var assetGuids = AssetDatabase.FindAssets($"t:{nameof(WindowCatalog)}");

            if (assetGuids.Length == 0)
            {
                Debug.LogWarning($"{nameof(WindowCatalog)} asset was not found." +
                                 $" Create it via {RootPath + CreationPath}");
                return;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            var catalog = AssetDatabase.LoadAssetAtPath<WindowCatalog>(assetPath);

            Selection.activeObject = catalog;
            EditorGUIUtility.PingObject(catalog);
        }
#endif
    }

    [CreateAssetMenu(fileName = nameof(WindowCatalog), menuName = WindowCatalogMenu.CreationPath)]
    internal sealed class WindowCatalog : ScriptableObject
    {
        [SerializeField] private List<WindowDefinition> _definitions = new();

        internal IReadOnlyList<WindowDefinition> Definitions => _definitions;
    }

    [Serializable]
    internal sealed class WindowDefinition
    {
        [SerializeField] private WindowBase _prefab;
        [SerializeField] private bool _isPopup;

        internal WindowBase Prefab => _prefab;
        internal bool IsPopup => _isPopup;
    }
}