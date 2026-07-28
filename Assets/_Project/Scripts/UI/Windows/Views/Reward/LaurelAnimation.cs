using DG.Tweening;
using LL.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Windows.Views.Reward
{
    [DisallowMultipleComponent]
    internal sealed class LaurelAnimation : RewardAnimation
    {
        [SerializeField] private Image _stem;
        [SerializeField] private Image[] _leaves;

        [Min(0f)] [SerializeField] private float _stemGrowSeconds = 0.48f;
        [Min(0f)] [SerializeField] private float _leafStartSeconds = 0.08f;
        [Min(0f)] [SerializeField] private float _leafDelaySeconds = 0.04f;
        [Min(0f)] [SerializeField] private float _leafUnfoldSeconds = 0.24f;

        protected override void BuildSequence(Sequence sequence)
        {
            sequence.Insert(
                0f,
                _stem.rectTransform.DOScaleY(1f, _stemGrowSeconds).SetEase(Ease.OutCubic));
            sequence.Insert(
                0f,
                _stem.DOFade(1f, _stemGrowSeconds * 0.35f).SetEase(Ease.OutQuad));

            for (var index = _leaves.Length - 1; index >= 0; index--)
            {
                var order = _leaves.Length - 1 - index;
                var delay = _leafStartSeconds + order * _leafDelaySeconds;
                InsertLeaf(sequence, _leaves[index], delay);
            }
        }

        protected override void ApplyHiddenState()
        {
            _stem.rectTransform.localScale = new Vector3(1f, 0f, 1f);
            _stem.SetAlpha(0f);

            foreach (var leaf in _leaves)
            {
                leaf.rectTransform.localScale = new Vector3(0f, 0f, 1f);
                leaf.SetAlpha(0f);
            }
        }

        private void InsertLeaf(Sequence sequence, Image leaf, float delay)
        {
            sequence.Insert(
                delay,
                leaf.rectTransform.DOScale(1f, _leafUnfoldSeconds).SetEase(Ease.OutBack, 1.1f));
            sequence.Insert(
                delay,
                leaf.DOFade(1f, _leafUnfoldSeconds * 0.45f).SetEase(Ease.OutQuad));
        }
    }
}