using LL.UI.Extensions;
using TMPro;
using UnityEngine;

namespace LL.UI
{
    [DisallowMultipleComponent]
    internal sealed class ProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _progressRect;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private string _labelFormat = "{0}%";

        internal void SetProgress(float progress)
        {
            progress = Mathf.Clamp01(progress);
            _progressRect.SetRange(0f, progress);
            _progressLabel.text = string.Format(_labelFormat, Mathf.RoundToInt(progress * 100f));
        }
    }
}