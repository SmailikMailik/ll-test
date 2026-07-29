using System;
using System.Collections.Generic;
using LL.Presentation.Collections;
using LL.Presentation.Icons;
using LL.Game.Rewards;
using UnityEngine;
using VContainer;

namespace LL.UI.Rewards
{
    [DisallowMultipleComponent]
    internal sealed class RewardContainerView : MonoBehaviour
    {
        [SerializeField] private RewardView _viewPrefab;
        [SerializeField] private RectTransform _viewContainer;

        private ReusableComponentCollection<RewardView> _views;
        private RewardIconProvider _iconProvider;

        [Inject]
        private void Construct(RewardIconProvider iconProvider)
        {
            _iconProvider = iconProvider ?? throw new ArgumentNullException(nameof(iconProvider));
        }

        private void Awake()
        {
            _views = new ReusableComponentCollection<RewardView>(_viewPrefab, _viewContainer);
        }

        internal void SetRewards(IReadOnlyList<IReward> rewards)
        {
            if (rewards is null)
            {
                Clear();
                return;
            }

            var rewardCount = rewards.Count;
            _views.EnsureCapacity(rewardCount);

            for (var index = 0; index < rewardCount; index++)
            {
                var reward = rewards[index];
                var icon = _iconProvider.GetIcon(reward);
                _views[index].UpdateView(icon, reward.Amount);
            }

            _views.SetActiveCount(rewardCount);
        }

        internal void Clear()
        {
            _views.Clear();
        }
    }
}