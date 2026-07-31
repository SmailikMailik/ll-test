using System;
using System.Collections.Generic;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.Windows.Configuration
{
    [CreateAssetMenu(fileName = nameof(WindowCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class WindowCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidEntries), "Assign every prefab and remove duplicate parameter types.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<WindowEntry> _entries = new();

        internal const string CreationPath = "LL/Window Catalog";

        private static readonly IDataValidator<IReadOnlyList<WindowEntry>> _validator = new WindowCatalogConfigValidator();

        internal IReadOnlyList<WindowEntry> Entries => _entries;

        private static bool HasValidEntries(List<WindowEntry> entries)
        {
            return ValidationRunner.IsValid(entries, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_entries, context);
        }
    }

    [Serializable]
    internal sealed class WindowEntry
    {
        [AssetsOnly]
        [HideLabel]
        [PropertyOrder(0)]
        [TableColumnWidth(280)]
        [SerializeField, Required] private WindowBase _prefab;

        [HideLabel]
        [PropertyOrder(2)]
        [TableColumnWidth(65, Resizable = false)]
        [SerializeField] private bool _popup;

        [ShowInInspector]
        [DisplayAsString]
        [HideLabel]
        [PropertyOrder(1)]
        [TableColumnWidth(180)]
        private string Parameters => _prefab == null ? "—" : _prefab.ParameterType.Name;

        internal WindowBase Prefab => _prefab;
        internal bool IsPopup => _popup;

        internal WindowDefinition ToDefinition() => new(Prefab, IsPopup);
    }
}