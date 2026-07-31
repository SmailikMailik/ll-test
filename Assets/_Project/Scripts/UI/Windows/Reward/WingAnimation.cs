using DG.Tweening;
using LL.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Windows.Reward
{
    [DisallowMultipleComponent]
    internal sealed class WingAnimation : RewardAnimation
    {
        [SerializeField] private Image _base;
        [SerializeField] private Image _topEdge;
        [SerializeField] private Image[] _feathers;

        [Min(0f)]
        [SerializeField] private float _baseFadeSeconds = 0.16f;

        [Min(0f)]
        [SerializeField] private float _topEdgeDelaySeconds = 0.03f;

        [Min(0f)]
        [SerializeField] private float _featherDelaySeconds = 0.035f;

        [Min(0f)]
        [SerializeField] private float _featherStartSeconds = 0.08f;

        [Min(0f)]
        [SerializeField] private float _unfoldSeconds = 0.3f;

        protected override void BuildSequence(Sequence sequence)
        {
            sequence.Insert(0f, _base.DOFade(1f, _baseFadeSeconds).SetEase(Ease.OutQuad));
            InsertUnfold(sequence, _topEdge, _topEdgeDelaySeconds);

            for (var index = 0; index < _feathers.Length; index++)
            {
                var delay = _featherStartSeconds + index * _featherDelaySeconds;
                InsertUnfold(sequence, _feathers[index], delay);
            }
        }

        protected override void ApplyHiddenState()
        {
            _base.SetAlpha(0f);
            SetHidden(_topEdge);

            foreach (var feather in _feathers)
                SetHidden(feather);
        }

        private void InsertUnfold(Sequence sequence, Image image, float delay)
        {
            sequence.Insert(
                delay,
                image.rectTransform.DOScaleX(1f, _unfoldSeconds).SetEase(Ease.OutBack, 1.15f));
            sequence.Insert(
                delay,
                image.DOFade(1f, _unfoldSeconds * 0.45f).SetEase(Ease.OutQuad));
        }

        private static void SetHidden(Image image)
        {
            image.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            image.SetAlpha(0f);
        }
    }
}