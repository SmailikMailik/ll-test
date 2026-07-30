using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LL.Presentation.Collections
{
    internal sealed class ReusableComponentCollection<TComponent>
        where TComponent : Component
    {
        internal TComponent this[int index] => _components[index];

        private readonly List<TComponent> _components = new();

        private readonly TComponent _prefab;
        private readonly Transform _parent;

        internal ReusableComponentCollection(TComponent prefab, Transform parent)
        {
            _prefab = prefab != null
                ? prefab
                : throw new ArgumentNullException(nameof(prefab));
            _parent = parent != null
                ? parent
                : throw new ArgumentNullException(nameof(parent));

            if (_parent.childCount > 0)
            {
                throw new ArgumentException(
                    "Reusable collection container must be empty.",
                    nameof(parent));
            }
        }

        internal void EnsureCapacity(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            while (_components.Count < count)
            {
                var component = Object.Instantiate(_prefab, _parent);
                component.gameObject.SetActive(false);
                _components.Add(component);
            }
        }

        internal void SetActiveCount(int count)
        {
            if (count < 0 || count > _components.Count)
                throw new ArgumentOutOfRangeException(nameof(count));

            for (var index = 0; index < _components.Count; index++)
                _components[index].gameObject.SetActive(index < count);
        }

        internal void Clear()
        {
            SetActiveCount(0);
        }
    }
}