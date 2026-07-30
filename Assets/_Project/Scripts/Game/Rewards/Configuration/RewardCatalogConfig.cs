using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Rewards.Configuration
{
    [CreateAssetMenu(fileName = nameof(RewardCatalogConfig), menuName = CreationPath)]
    internal sealed class RewardCatalogConfig :
        ScriptableObject,
        IDataLoader<RewardCatalog>,
        IValidationSource
    {
        [ValidateInput(nameof(HasValidRewards), "Reward data is invalid.")]
        [SerializeField] private RewardEntry[] _rewards;

        internal const string CreationPath = "LL/Game Data/Reward Catalog";

        private static readonly IDataValidator<RewardEntry[]> _validator =
            new RewardCatalogConfigValidator();

        internal IReadOnlyList<RewardEntry> Rewards => _rewards;

        public RewardCatalog Load()
        {
            ValidationRunner.EnsureValid(this, nameof(_rewards));

            return new RewardCatalog(_rewards.Select(entry => entry.ToReward()));
        }

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

        internal Reward ToReward() =>
            new(Id, Items.Select(item => item.ToItemAmount()));
    }

    [Serializable]
    internal sealed class RewardItemEntry
    {
        [SerializeField] private string _id;
        [SerializeField, MinValue(1)] private int _amount = 1;

        internal ItemId Id => new(_id);
        internal int Amount => _amount;

        internal ItemAmount ToItemAmount() => new(Id, Amount);
    }
}