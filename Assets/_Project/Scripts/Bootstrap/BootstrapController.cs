using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LL.Bootstrap
{
    [DisallowMultipleComponent]
    internal sealed class BootstrapController : MonoBehaviour
    {
        [SerializeField] private Image _progressFill;
        [SerializeField] private Text _loadingLabel;
        [SerializeField] private string _targetSceneName = "Main";

        [Min(0f)]
        [SerializeField] private float _minimumDisplaySeconds = 0.75f;

        private const float ReadyProgress = 0.9f;

        private void Awake()
        {
            if (_progressFill != null && _loadingLabel != null)
                return;

            var view = BootstrapLoadingView.Create();
            _progressFill = view.ProgressFill;
            _loadingLabel = view.LoadingLabel;
        }

        private IEnumerator Start()
        {
            if (_progressFill == null)
                throw new InvalidOperationException("Bootstrap progress fill is not assigned.");

            if (_loadingLabel == null)
                throw new InvalidOperationException("Bootstrap loading label is not assigned.");

            if (string.IsNullOrWhiteSpace(_targetSceneName))
                throw new InvalidOperationException("Bootstrap target scene name is not assigned.");

            SetProgress(0f);
            yield return null;

            var startedAt = Time.realtimeSinceStartup;
            var operation = SceneManager.LoadSceneAsync(_targetSceneName, LoadSceneMode.Single);

            if (operation == null)
                throw new InvalidOperationException($"Failed to start loading scene '{_targetSceneName}'.");

            operation.allowSceneActivation = false;

            while (operation.isDone is false)
            {
                var loadProgress = Mathf.Clamp01(operation.progress / ReadyProgress);
                var timeProgress = _minimumDisplaySeconds <= 0f
                    ? 1f
                    : Mathf.Clamp01((Time.realtimeSinceStartup - startedAt) / _minimumDisplaySeconds);

                SetProgress(Mathf.Min(loadProgress, timeProgress));

                if (loadProgress >= 1f && timeProgress >= 1f)
                {
                    SetProgress(1f);
                    yield return null;
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }

        private void SetProgress(float progress)
        {
            _progressFill.fillAmount = progress;
            _loadingLabel.text = $"LOADING {Mathf.RoundToInt(progress * 100f)}%";
        }
    }
}