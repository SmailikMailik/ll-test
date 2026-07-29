using System.Linq;
using System;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace LL.Game.Rewards.Configuration
{
    [CreateAssetMenu(fileName = nameof(RewardCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RewardCatalogConfig :
        ScriptableObject,
        IDataLoader<RewardCatalog>,
        IValidationSource
    {
        [FormerlySerializedAs("_bundles")]
        [ValidateInput(nameof(HasValidRewards), "Reward data is invalid.")]
        [ListDrawerSettings(ShowFoldout = false, ShowPaging = false)]
        [SerializeField] private RewardEntry[] _rewards;

        internal const string CreationPath = "LL/Game Data/Reward Catalog";

        private static readonly IDataValidator<RewardEntry[]> _validator =
            new RewardCatalogConfigValidator();

        internal RewardEntry[] Rewards => _rewards;

        public RewardCatalog Load()
        {
            ValidationRunner.EnsureValid(this, nameof(_rewards));

            return new RewardCatalog(_rewards?.Select(entry => entry.ToReward()));
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
        [HorizontalGroup("Columns")]
        [BoxGroup("Columns/Reward")]
        [LabelText("ID")]
        [SerializeField] private string _id;

        [FormerlySerializedAs("_rewards")]
        [BoxGroup("Columns/Items")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false, HideToolbar = false)]
        [SerializeField] private RewardItemEntry[] _items;

        internal RewardId Id => new(_id);
        internal RewardItemEntry[] Items => _items;

        internal Reward ToReward()
        {
            return new Reward(
                Id,
                Items?.Select(item => item.ToItemAmount()));
        }
    }

    [Serializable]
    internal sealed class RewardItemEntry
    {
        [TableColumnWidth(120, Resizable = false)]
        [HideLabel]
        [SerializeField] private string _id;

        [TableColumnWidth(80, Resizable = false)]
        [HideLabel]
        [MinValue(1)]
        [SerializeField] private int _amount = 1;

        internal ItemId Id => new(_id);
        internal int Amount => _amount;

        internal ItemAmount ToItemAmount() => new(Id, Amount);
    }
}