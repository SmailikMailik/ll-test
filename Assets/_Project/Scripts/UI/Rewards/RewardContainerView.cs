using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Presentation.Icons;
using UnityEngine;
using VContainer;

namespace LL.UI.Rewards
{
    [DisallowMultipleComponent]
    internal sealed class RewardContainerView : MonoBehaviour
    {
        [SerializeField] private RewardView _viewPrefab;
        [SerializeField] private RectTransform _viewContainer;

        private ReusableComponentCollection<RewardView> _views;
        private IconCatalog<ItemId> _icons;

        [Inject]
        private void Construct(IconCatalog<ItemId> icons)
        {
            _icons = icons ?? throw new ArgumentNullException(nameof(icons));
        }

        private void Awake()
        {
            _views = new ReusableComponentCollection<RewardView>(_viewPrefab, _viewContainer);
        }

        internal void SetItems(IReadOnlyList<ItemAmount> items)
        {
            if (items is null)
            {
                Clear();
                return;
            }

            _views.EnsureCapacity(items.Count);

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];

                if (_icons.TryGetIcon(item.Id, out var icon) is false)
                    throw new KeyNotFoundException($"Missing reward item icon: {item.Id}.");

                _views[index].UpdateView(icon, item.Amount);
            }

            _views.SetActiveCount(items.Count);
        }

        internal void Clear()
        {
            _views.Clear();
        }
    }
}