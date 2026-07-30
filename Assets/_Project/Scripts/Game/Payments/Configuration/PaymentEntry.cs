using System;
using LL.Game.Items;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Payments.Configuration
{
    [Serializable]
    [InlineProperty]
    internal sealed class PaymentEntry
    {
        [HorizontalGroup("Payment")]
        [LabelText("Item ID")]
        [SerializeField] private string _itemId;

        [HorizontalGroup("Payment")]
        [LabelText("Amount")]
        [MinValue(1)]
        [SerializeField] private int _amount = 1;

        internal ItemId ItemId => new(_itemId);
        internal int Amount => _amount;

        internal Payment ToPayment() => new(ItemId, Amount);
    }
}