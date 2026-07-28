using System;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Identifiers;
using LL.Rewards.Models;
using UnityEngine;
using VContainer;

namespace LL.Presentation.Icons
{
    internal sealed class RewardIconProvider
    {
        private readonly IIconProvider<ItemId> _itemIconProvider;
        private readonly IIconProvider<CardId> _cardIconProvider;

        [Inject]
        internal RewardIconProvider(
            IIconProvider<ItemId> itemIconProvider,
            IIconProvider<CardId> cardIconProvider)
        {
            _itemIconProvider = itemIconProvider ?? throw new ArgumentNullException(nameof(itemIconProvider));
            _cardIconProvider = cardIconProvider ?? throw new ArgumentNullException(nameof(cardIconProvider));
        }

        internal Sprite GetIcon(IReward reward) => reward switch
        {
            ItemReward item => GetIcon(_itemIconProvider, item.ItemId),
            CardReward card => GetIcon(_cardIconProvider, card.CardId),
            _ => null
        };

        private static Sprite GetIcon<TId>(IIconProvider<TId> provider, TId id)
            where TId : struct, IIdentifier
        {
            return provider.TryGetIcon(id, out var icon) ? icon : null;
        }
    }
}