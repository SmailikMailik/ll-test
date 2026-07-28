using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Decorations
{
    [DisallowMultipleComponent]
    internal sealed class LaurelAnimation : MonoBehaviour
    {
        [SerializeField] private Image _stem;
        [SerializeField] private Image[] _leaves;

        [Min(0f)] [SerializeField] private float _stemGrowSeconds = 0.48f;
        [Min(0f)] [SerializeField] private float _leafStartSeconds = 0.08f;
        [Min(0f)] [SerializeField] private float _leafDelaySeconds = 0.04f;
        [Min(0f)] [SerializeField] private float _leafUnfoldSeconds = 0.24f;

        private Sequence _sequence;

        private void Awake()
        {
            ApplyHiddenState();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        internal void Play()
        {
            ResetView();

            _sequence = DOTween.Sequence().SetUpdate(true);
            _sequence.Insert(
                0f,
                _stem.rectTransform.DOScaleY(1f, _stemGrowSeconds).SetEase(Ease.OutCubic));
            _sequence.Insert(
                0f,
                _stem.DOFade(1f, _stemGrowSeconds * 0.35f).SetEase(Ease.OutQuad));

            for (var index = _leaves.Length - 1; index >= 0; index--)
            {
                var order = _leaves.Length - 1 - index;
                var delay = _leafStartSeconds + order * _leafDelaySeconds;
                InsertLeaf(_leaves[index], delay);
            }

            _sequence.Play();
        }

        internal void Complete()
        {
            StopAnimation();
            ApplyVisibleState();
        }

        internal void ResetView()
        {
            StopAnimation();
            ApplyHiddenState();
        }

        private void InsertLeaf(Image leaf, float delay)
        {
            _sequence.Insert(
                delay,
                leaf.rectTransform.DOScale(1f, _leafUnfoldSeconds).SetEase(Ease.OutBack, 1.1f));
            _sequence.Insert(
                delay,
                leaf.DOFade(1f, _leafUnfoldSeconds * 0.45f).SetEase(Ease.OutQuad));
        }

        private void ApplyHiddenState()
        {
            _stem.rectTransform.localScale = new Vector3(1f, 0f, 1f);
            SetAlpha(_stem, 0f);

            foreach (var leaf in _leaves)
            {
                leaf.rectTransform.localScale = new Vector3(0f, 0f, 1f);
                SetAlpha(leaf, 0f);
            }
        }

        private void ApplyVisibleState()
        {
            _stem.rectTransform.localScale = Vector3.one;
            SetAlpha(_stem, 1f);

            foreach (var leaf in _leaves)
            {
                leaf.rectTransform.localScale = Vector3.one;
                SetAlpha(leaf, 1f);
            }
        }

        private void StopAnimation()
        {
            _sequence?.Kill();
            _sequence = null;
        }

        private static void SetAlpha(Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}