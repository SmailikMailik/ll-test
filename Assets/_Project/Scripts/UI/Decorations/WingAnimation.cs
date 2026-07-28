using DG.Tweening;
using LL.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Decorations
{
    [DisallowMultipleComponent]
    internal sealed class WingAnimation : MonoBehaviour
    {
        [SerializeField] private Image _base;
        [SerializeField] private Image _topEdge;
        [SerializeField] private Image[] _feathers;

        [Min(0f)] [SerializeField] private float _baseFadeSeconds = 0.16f;
        [Min(0f)] [SerializeField] private float _topEdgeDelaySeconds = 0.03f;
        [Min(0f)] [SerializeField] private float _featherDelaySeconds = 0.035f;
        [Min(0f)] [SerializeField] private float _featherStartSeconds = 0.08f;
        [Min(0f)] [SerializeField] private float _unfoldSeconds = 0.3f;

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
            _sequence.Insert(0f, _base.DOFade(1f, _baseFadeSeconds).SetEase(Ease.OutQuad));
            InsertUnfold(_topEdge, _topEdgeDelaySeconds);

            for (var index = 0; index < _feathers.Length; index++)
            {
                var delay = _featherStartSeconds + index * _featherDelaySeconds;
                InsertUnfold(_feathers[index], delay);
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

        private void InsertUnfold(Image image, float delay)
        {
            _sequence.Insert(
                delay,
                image.rectTransform.DOScaleX(1f, _unfoldSeconds).SetEase(Ease.OutBack, 1.15f));
            _sequence.Insert(
                delay,
                image.DOFade(1f, _unfoldSeconds * 0.45f).SetEase(Ease.OutQuad));
        }

        private void ApplyHiddenState()
        {
            _base.SetAlpha(0f);
            SetHidden(_topEdge);

            foreach (var feather in _feathers)
                SetHidden(feather);
        }

        private void ApplyVisibleState()
        {
            _base.SetAlpha(1f);
            SetVisible(_topEdge);

            foreach (var feather in _feathers)
                SetVisible(feather);
        }

        private void StopAnimation()
        {
            _sequence?.Kill();
            _sequence = null;
        }

        private static void SetHidden(Image image)
        {
            image.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            image.SetAlpha(0f);
        }

        private static void SetVisible(Image image)
        {
            image.rectTransform.localScale = Vector3.one;
            image.SetAlpha(1f);
        }
    }
}