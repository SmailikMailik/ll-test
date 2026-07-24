using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LL.UI.Windows
{
#if UNITY_EDITOR
    internal static class WindowsSettingsMenu
    {
        internal const string RootPath = "Tools/";
        internal const string CreationPath = "LL/Windows Settings";

        [MenuItem(RootPath + CreationPath)]
        private static void SelectObject()
        {
            var assetGuids = AssetDatabase.FindAssets($"t:{nameof(WindowsSettings)}");

            if (assetGuids.Length == 0)
            {
                Debug.LogWarning($"{nameof(WindowsSettings)} asset was not found." +
                                 $" Create it via {RootPath + CreationPath}");
                return;
            }

            var assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
            var settings = AssetDatabase.LoadAssetAtPath<WindowsSettings>(assetPath);

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }
    }
#endif

    [CreateAssetMenu(fileName = nameof(WindowsSettings), menuName = WindowsSettingsMenu.CreationPath)]
    internal sealed class WindowsSettings : SerializedScriptableObject
    {
        [field: SerializeField] internal Dictionary<WindowType, WindowData> Items { get; private set; } = new();
    }

    [Serializable]
    internal sealed class WindowData
    {
        [field: SerializeField] internal WindowBase Prefab { get; private set; }
        [field: SerializeField] internal bool IsPopup { get; private set; }
    }

    internal enum WindowType : byte
    {
        Unknown = 0,
        Modal = 1,
        Upgrade = 2
    }
}