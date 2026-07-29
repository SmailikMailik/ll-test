using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Identifiers;
using LL.Infrastructure.Loading;
using LL.Game.Rewards;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Rewards.Configuration
{
    [CreateAssetMenu(fileName = nameof(RewardBundleCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RewardBundleCatalogConfig : ScriptableObject, IDataLoader<RewardBundleCatalog>
    {
        [ValidateInput(nameof(HasValidBundleIds), "Reward bundle IDs must be non-empty and unique.")]
        [ListDrawerSettings(ShowFoldout = false, ShowPaging = false)]
        [SerializeField] private RewardBundleEntry[] _bundles;

        internal const string CreationPath = "LL/Game Data/Reward Bundle Catalog";

        public RewardBundleCatalog Load()
        {
            IdentifierCatalogValidator.EnsureValidIds(
                _bundles,
                entry => entry.Id,
                nameof(RewardBundleCatalogConfig),
                nameof(_bundles));

            return new RewardBundleCatalog(_bundles?.Select(entry => entry.ToBundle()));
        }

        private static bool HasValidBundleIds(RewardBundleEntry[] bundles)
        {
            return IdentifierCatalogValidator.HasValidIds(bundles, entry => entry.Id);
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

        internal RewardBundle ToBundle()
        {
            return new RewardBundle(
                Id,
                _grantMode,
                _rewards?.Select(reward => reward.ToReward()));
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

        internal ItemReward ToReward() => new(new ItemId(_id), _amount);
    }
}