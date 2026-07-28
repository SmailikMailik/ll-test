using System;
using System.Collections.Generic;
using LL.Extensions;
using LL.Rewards;
using UnityEngine;
using VContainer;

namespace LL.UI.Rewards
{
    [DisallowMultipleComponent]
    internal sealed class RewardLayout : MonoBehaviour
    {
        [SerializeField] private RewardView _rewardPrefab;
        [SerializeField] private RectTransform _content;

        private RewardIconProvider _iconProvider;

        [Inject]
        private void Construct(RewardIconProvider iconProvider)
        {
            _iconProvider = iconProvider ?? throw new ArgumentNullException(nameof(iconProvider));
        }

        internal void SetRewards(IReadOnlyList<IReward> rewards)
        {
            Clear();

            if (rewards is null)
                return;

            foreach (var reward in rewards)
                CreateReward(reward);
        }

        internal void Clear()
        {
            _content.DestroyAllChildren();
        }

        private void CreateReward(IReward reward)
        {
            var view = Instantiate(_rewardPrefab, _content);
            view.UpdateView(reward, _iconProvider);
        }
    }
}