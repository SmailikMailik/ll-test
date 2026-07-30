using UnityEditor;
using UnityEngine;

namespace LLEditor
{
    internal static class ProjectAssetSelector
    {
        internal static void Select<TAsset>(string creationPath)
            where TAsset : Object
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
    }
}