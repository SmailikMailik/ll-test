using System;
using LL.Game.Items;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Payments.Configuration
{
    [Serializable]
    internal sealed class PaymentEntry
    {
        [SerializeField] private string _itemId;
        [SerializeField, MinValue(1)] private int _amount = 1;

        internal ItemId ItemId => new(_itemId);
        internal int Amount => _amount;

        internal Payment ToPayment() => new(ItemId, Amount);
    }
}