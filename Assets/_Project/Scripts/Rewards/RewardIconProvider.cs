using LL.Game.Cards;
using LL.Game.Currencies;
using LL.Game.Items;
using LL.Identifiers;
using LL.Presentation.Icons;
using UnityEngine;
using VContainer;

namespace LL.Rewards
{
    internal sealed class RewardIconProvider
    {
        private readonly IIconProvider<CurrencyId> _currencyIconProvider;
        private readonly IIconProvider<ItemId> _itemIconProvider;
        private readonly IIconProvider<CardId> _cardIconProvider;

        [Inject]
        internal RewardIconProvider(
            IIconProvider<CurrencyId> currencyIconProvider,
            IIconProvider<ItemId> itemIconProvider,
            IIconProvider<CardId> cardIconProvider)
        {
            _currencyIconProvider = currencyIconProvider;
            _itemIconProvider = itemIconProvider;
            _cardIconProvider = cardIconProvider;
        }

        internal Sprite GetIcon(IReward reward)
        {
            return reward switch
            {
                CurrencyReward currency => GetIcon(_currencyIconProvider, currency.CurrencyId),
                ItemReward item => GetIcon(_itemIconProvider, item.ItemId),
                CardReward card => GetIcon(_cardIconProvider, card.CardId),
                _ => null
            };
        }

        private static Sprite GetIcon<TId>(IIconProvider<TId> provider, TId id)
            where TId : struct, IIdentifier
        {
            return provider.TryGetIcon(id, out var icon) ? icon : null;
        }
    }
}