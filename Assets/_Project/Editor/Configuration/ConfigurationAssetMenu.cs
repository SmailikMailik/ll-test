using UnityEditor;
using UnityEngine;

namespace LLEditor.Configuration
{
    internal static class ConfigurationAssetMenu
    {
        internal const string RootPath = "Tools/LL/";

        internal static void Select<TAsset>(string creationPath) where TAsset : Object
        {
            var assetGuids = AssetDatabase.FindAssets($"t:{typeof(TAsset).Name}");

            if (assetGuids.Length == 0)
            {
                Debug.LogWarning(
                    $"{typeof(TAsset).Name} asset was not found. " +
                    $"Create it via Assets/Create/{creationPath}");
                return;
            }

            if (assetGuids.Length > 1)
            {
                Debug.LogWarning(
                    $"Multiple {typeof(TAsset).Name} assets were found. " +
                    "The first one will be selected.");
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            var asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);

            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        internal static void SelectAll<TAsset>(string creationPath) where TAsset : Object
        {
            var assetGuids = AssetDatabase.FindAssets($"t:{typeof(TAsset).Name}");

            if (assetGuids.Length == 0)
            {
                Debug.LogWarning(
                    $"{typeof(TAsset).Name} assets were not found. " +
                    $"Create them via Assets/Create/{creationPath}");
                return;
            }

            var assets = new Object[assetGuids.Length];

            for (var index = 0; index < assetGuids.Length; index++)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[index]);
                assets[index] = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);
            }

            Selection.objects = assets;
            EditorGUIUtility.PingObject(assets[0]);
        }
    }
}