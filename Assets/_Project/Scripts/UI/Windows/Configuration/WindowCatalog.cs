using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.Windows.Configuration
{
    [CreateAssetMenu(fileName = nameof(WindowCatalog), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class WindowCatalog : ScriptableObject
    {
        internal const string CreationPath = "LL/Window Catalog";

        [ValidateInput(nameof(HasValidDefinitions), "Assign every prefab and remove duplicate parameter types.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<WindowDefinition> _definitions = new();

        internal IReadOnlyList<WindowDefinition> Definitions => _definitions;

        private static bool HasValidDefinitions(List<WindowDefinition> definitions)
        {
            if (definitions == null)
                return true;

            var parameterTypes = new HashSet<Type>();

            foreach (var definition in definitions)
            {
                if (definition?.Prefab == null)
                    return false;

                if (parameterTypes.Add(definition.Prefab.ParameterType) is false)
                    return false;
            }

            return true;
        }
    }

    [Serializable]
    internal sealed class WindowDefinition
    {
        [Required]
        [AssetsOnly]
        [HideLabel]
        [PropertyOrder(0)]
        [TableColumnWidth(280)]
        [SerializeField] private WindowBase _prefab;

        [ShowInInspector]
        [DisplayAsString]
        [HideLabel]
        [PropertyOrder(1)]
        [TableColumnWidth(180)]
        private string Parameters => _prefab == null ? "—" : _prefab.ParameterType.Name;

        [HideLabel]
        [PropertyOrder(2)]
        [TableColumnWidth(65, Resizable = false)]
        [SerializeField] private bool _popup;

        internal WindowBase Prefab => _prefab;
        internal bool IsPopup => _popup;
    }
}