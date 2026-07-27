using System;
using System.Collections.Generic;
using System.Linq;
using LL.Loading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Rewards.Ranks
{
    [CreateAssetMenu(fileName = nameof(RankRewardCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class RankRewardCatalogConfig : ScriptableObject, IDataLoader<RankRewardCatalog>
    {
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private RankRewardEntry[] _rewards;

        internal const string CreationPath = "LL/Rewards/Rank Reward Catalog";

        public RankRewardCatalog Load()
        {
            return new RankRewardCatalog(_rewards?.Select(entry => entry.ToPair()));
        }
    }

    [Serializable]
    internal sealed class RankRewardEntry
    {
        [LabelText("Rank")]
        [MinValue(1)]
        [SerializeField] private int _rank = 1;

        [LabelText("Reward Bundle ID")]
        [SerializeField] private string _rewardBundleId;

        internal KeyValuePair<int, RewardBundleId> ToPair()
        {
            return new KeyValuePair<int, RewardBundleId>(
                _rank,
                new RewardBundleId(_rewardBundleId));
        }
    }
}