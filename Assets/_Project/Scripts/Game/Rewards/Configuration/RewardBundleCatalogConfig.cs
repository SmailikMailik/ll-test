using System;
using System.Linq;
using LL.Game.Data.Validation;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Rewards.Configuration
{
    [CreateAssetMenu(fileName = nameof(RewardBundleCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RewardBundleCatalogConfig :
        ScriptableObject,
        IDataLoader<RewardBundleCatalog>,
        IValidationSource
    {
        [ValidateInput(nameof(HasValidBundles), "Reward bundle data is invalid.")]
        [ListDrawerSettings(ShowFoldout = false, ShowPaging = false)]
        [SerializeField] private RewardBundleEntry[] _bundles;

        internal const string CreationPath = "LL/Game Data/Reward Bundle Catalog";

        private static readonly IDataValidator<RewardBundleEntry[]> _validator =
            new RewardBundleCatalogConfigValidator();

        internal RewardBundleEntry[] Bundles => _bundles;

        public RewardBundleCatalog Load()
        {
            var result = ValidationRunner.Run(this);
            ValidationResultGuard.EnsureValid(result, nameof(_bundles));

            return new RewardBundleCatalog(_bundles?.Select(entry => entry.ToBundle()));
        }

        private static bool HasValidBundles(RewardBundleEntry[] bundles)
        {
            return ValidationRunner.Run(bundles, _validator).IsValid;
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_bundles, context);
        }
    }

    [Serializable]
    internal sealed class RewardBundleEntry
    {
        [HorizontalGroup("Columns")]
        [BoxGroup("Columns/Bundle")]
        [LabelText("ID")]
        [SerializeField] private string _id;

        [BoxGroup("Columns/Bundle")]
        [LabelText("Grant mode")]
        [SerializeField] private RewardGrantMode _grantMode;

        [BoxGroup("Columns/Rewards")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false, HideToolbar = false)]
        [SerializeField] private ItemRewardEntry[] _rewards;

        internal RewardBundleId Id => new(_id);
        internal RewardGrantMode GrantMode => _grantMode;
        internal ItemRewardEntry[] Rewards => _rewards;

        internal RewardBundle ToBundle()
        {
            return new RewardBundle(
                Id,
                GrantMode,
                Rewards?.Select(reward => reward.ToReward()));
        }
    }

    [Serializable]
    internal sealed class ItemRewardEntry
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

        internal ItemReward ToReward() => new(Id, Amount);
    }
}