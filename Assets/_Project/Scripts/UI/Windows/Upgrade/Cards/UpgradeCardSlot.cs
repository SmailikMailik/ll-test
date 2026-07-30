using System;
using LL.Game.Items;
using UnityEngine;

namespace LL.UI.Windows.Upgrade.Cards
{
    [Serializable]
    internal sealed class UpgradeCardSlot
    {
        [SerializeField] private UpgradeCardView _view;
        [SerializeField] private string _cardId;

        internal UpgradeCardView View => _view;
        internal ItemId Id => new(_cardId);
    }
}