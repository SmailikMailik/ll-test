using LL.Extensions;
using UnityEngine;

namespace LL.UI
{
    [DisallowMultipleComponent]
    internal sealed class PredictedProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _currentProgressRect;
        [SerializeField] private RectTransform _predictedProgressRect;

        [SerializeField] private RectTransform _currentCap;
        [SerializeField] private RectTransform _predictGlow;

        private const float MinimumVisibleProgress = 0.0001f;

        internal void SetProgress(float currentProgress, float predictedProgress)
        {
            currentProgress = Mathf.Clamp01(currentProgress);
            predictedProgress = Mathf.Clamp(predictedProgress, currentProgress, 1f);

            var hasCurrentProgress = currentProgress > MinimumVisibleProgress;
            var hasPrediction = predictedProgress - currentProgress > MinimumVisibleProgress;

            _currentProgressRect.gameObject.SetActive(hasCurrentProgress);
            _currentProgressRect.SetRange(0f, currentProgress);

            _currentCap.gameObject.SetActive(hasCurrentProgress);
            _currentCap.anchorMin = new Vector2(currentProgress, 0.5f);
            _currentCap.anchorMax = new Vector2(currentProgress, 0.5f);
            _currentCap.anchoredPosition = Vector2.zero;

            _predictedProgressRect.gameObject.SetActive(hasPrediction);
            _predictedProgressRect.SetRange(currentProgress, predictedProgress);

            _predictGlow.gameObject.SetActive(hasPrediction);
            _predictGlow.SetRange(currentProgress, predictedProgress);
        }
    }
}