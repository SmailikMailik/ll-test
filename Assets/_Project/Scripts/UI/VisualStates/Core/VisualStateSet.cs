using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.UI.VisualStates.Core
{
    [CreateAssetMenu(fileName = nameof(VisualStateSet), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class VisualStateSet : ScriptableObject
    {
        internal const string CreationPath = "LL/UI/Visual State Set";

        [InfoBox("The first state is used as the default state.")]
        [ValidateInput(nameof(HasValidStates), "Add at least one state. State names must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private List<StateDefinition> _states = new();

        internal IReadOnlyList<StateDefinition> States => _states;
        internal VisualStateId DefaultState => _states is { Count: > 0 } ? _states[0].Id : default;

        internal bool Contains(VisualStateId state) => Find(state) != null;

        internal string GetName(VisualStateId state) => Find(state)?.Name ?? string.Empty;

        private void OnValidate()
        {
            _states ??= new List<StateDefinition>();

            var identifiers = new HashSet<VisualStateId>();

            for (var index = 0; index < _states.Count; index++)
            {
                var definition = _states[index] ?? new StateDefinition();
                definition.Normalize(identifiers);
                identifiers.Add(definition.Id);
                _states[index] = definition;
            }
        }

        private static bool HasValidStates(List<StateDefinition> states)
        {
            if (states == null || states.Count == 0)
                return false;

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var definition in states)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Name))
                    return false;

                if (names.Add(definition.Name.Trim()) is false)
                    return false;
            }

            return true;
        }

        private StateDefinition Find(VisualStateId state)
        {
            if (state.IsEmpty || _states == null)
                return null;

            foreach (var definition in _states)
            {
                if (definition != null && definition.Id == state)
                    return definition;
            }

            return null;
        }

        [Serializable]
        internal sealed class StateDefinition
        {
            [HideInInspector]
            [SerializeField] private VisualStateId _id;

            [HideLabel]
            [SerializeField] private string _name;

            internal VisualStateId Id => _id;
            internal string Name => _name;

            internal void Normalize(ISet<VisualStateId> existingIdentifiers)
            {
                _name = _name?.Trim();

                if (_id.IsEmpty || existingIdentifiers.Contains(_id))
                    _id = VisualStateId.Create();
            }
        }
    }
}