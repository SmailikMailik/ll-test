using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Rewards.Configuration
{
    [CreateAssetMenu(fileName = nameof(RewardCatalogConfig), menuName = CreationPath)]
    internal sealed class RewardCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidRewards), "Reward data is invalid.")]
        [SerializeField] private RewardEntry[] _rewards;

        internal const string CreationPath = "LL/Game Data/Reward Catalog";

        private static readonly IDataValidator<RewardEntry[]> _validator = new RewardCatalogConfigValidator();

        internal IReadOnlyList<RewardEntry> Rewards => _rewards;

        private static bool HasValidRewards(RewardEntry[] rewards)
        {
            return ValidationRunner.IsValid(rewards, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_rewards, context);
        }
    }

    [Serializable]
    internal sealed class RewardEntry
    {
        [SerializeField] private string _id;
        [SerializeField] private RewardItemEntry[] _items;

        internal RewardId Id => new(_id);
        internal IReadOnlyList<RewardItemEntry> Items => _items;
    }

    [Serializable]
    internal sealed class RewardItemEntry
    {
        [SerializeField] private string _id;

        [MinValue(1)]
        [SerializeField] private int _amount = 1;

        internal ItemId Id => new(_id);
        internal int Amount => _amount;
    }
}