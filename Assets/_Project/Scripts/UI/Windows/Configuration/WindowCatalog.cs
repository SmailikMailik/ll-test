using System;
using System.Collections.Generic;
using UnityEngine;

namespace LL.UI.Windows.Configuration
{
    [CreateAssetMenu(fileName = nameof(WindowCatalog), menuName = WindowCatalog.CreationPath)]
    internal sealed class WindowCatalog : ScriptableObject
    {
        internal const string CreationPath = "LL/Window Catalog";

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