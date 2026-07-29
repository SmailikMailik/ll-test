using System;
using LL.Game.Items;
using LL.Rewards.Models;
using UnityEngine;
using VContainer;

namespace LL.Presentation.Icons
{
    internal sealed class RewardIconProvider
    {
        private readonly IIconProvider<ItemId> _itemIconProvider;

        [Inject]
        internal RewardIconProvider(IIconProvider<ItemId> itemIconProvider)
        {
            _itemIconProvider = itemIconProvider ?? throw new ArgumentNullException(nameof(itemIconProvider));
        }

        internal Sprite GetIcon(IReward reward) => reward switch
        {
            ItemReward item => GetIcon(item.ItemId),
            _ => null
        };

        private Sprite GetIcon(ItemId id)
        {
            return _itemIconProvider.TryGetIcon(id, out var icon) ? icon : null;
        }
    }
}