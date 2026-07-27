using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LL.Coroutines;
using LL.Extensions;
using LL.Rewards;
using UnityEngine;
using VContainer;

namespace LL.UI
{
    internal sealed class RewardsContainer : MonoBehaviour
    {
        [SerializeField] private RewardView _viewTemplate;
        [SerializeField] private RectTransform _container;

        private RewardIconProvider _iconProvider;

        [Inject]
        private void Construct(RewardIconProvider iconProvider)
        {
            _iconProvider = iconProvider;
        }

        internal void ShowRewardsDelayed(IReadOnlyList<IReward> rewards)
        {
            StartCoroutine(ShowRewardsDelayedInner(rewards));
        }

        internal void ShowRewardsInstantly(params IReward[] rewards)
        {
            ClearRewards();

            foreach (var reward in rewards)
                CreateReward(reward);
        }

        internal void ClearRewards()
        {
            _container.DestroyAllChildren();
        }

        private IEnumerator ShowRewardsDelayedInner(IReadOnlyList<IReward> rewards)
        {
            ClearRewards();

            if (rewards is null)
                yield break;

            yield return Yielders.WaitForSeconds(0.1f);

            foreach (var reward in rewards)
            {
                yield return Yielders.WaitForSeconds(0.2f);

                var newView = CreateReward(reward);

                newView.transform
                    .DOScale(newView.transform.localScale, 0.2f)
                    .From(Vector3.zero)
                    .SetEase(Ease.OutBack)
                    .Play();
            }
        }

        private RewardView CreateReward(IReward reward)
        {
            var newView = Instantiate(_viewTemplate, _container);
            newView.UpdateView(reward, _iconProvider);

            return newView;
        }
    }
}