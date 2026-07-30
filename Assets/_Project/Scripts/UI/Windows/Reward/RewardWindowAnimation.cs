using DG.Tweening;
using UnityEngine;

namespace LL.UI.Windows.Reward
{
    [DisallowMultipleComponent]
    internal sealed class RewardWindowAnimation : RewardAnimation
    {
        [SerializeField] private WingAnimation[] _wings;
        [SerializeField] private LaurelAnimation[] _laurels;
        [SerializeField] private CanvasGroup _titleGroup;
        [SerializeField] private CanvasGroup _heroGroup;
        [SerializeField] private CanvasGroup _rankGroup;
        [SerializeField] private CanvasGroup _contentGroup;
        [SerializeField] private CanvasGroup _continueGroup;
        [SerializeField] private RotationAnimation _effect;
        [SerializeField] private RectTransform _rank;

        protected override void BuildSequence(Sequence sequence)
        {
            sequence.Insert(0.05f, _titleGroup.DOFade(1f, 0.2f).SetEase(Ease.OutQuad));
            sequence.Insert(0.18f, _heroGroup.DOFade(1f, 0.3f).SetEase(Ease.OutQuad));
            sequence.Insert(0.28f, _rankGroup.DOFade(1f, 0.2f).SetEase(Ease.OutQuad));
            sequence.Insert(0.28f, _rank.DOScale(1f, 0.32f).SetEase(Ease.OutBack, 1.2f));
            sequence.Insert(0.5f, _contentGroup.DOFade(1f, 0.25f).SetEase(Ease.OutQuad));
            sequence.Insert(0.78f, _continueGroup.DOFade(1f, 0.25f).SetEase(Ease.OutQuad));
        }

        protected override void ApplyHiddenState()
        {
            _titleGroup.alpha = 0f;
            _heroGroup.alpha = 0f;
            _rankGroup.alpha = 0f;
            _contentGroup.alpha = 0f;
            _continueGroup.alpha = 0f;
            _rank.localScale = Vector3.one * 0.65f;
        }

        protected override void PlayNestedAnimations()
        {
            foreach (var wing in _wings)
                wing.Play();

            foreach (var laurel in _laurels)
                laurel.Play();

            _effect.Play();
        }

        protected override void ResetNestedAnimations()
        {
            _effect.ResetView();

            foreach (var wing in _wings)
                wing.ResetView();

            foreach (var laurel in _laurels)
                laurel.ResetView();
        }
    }
}