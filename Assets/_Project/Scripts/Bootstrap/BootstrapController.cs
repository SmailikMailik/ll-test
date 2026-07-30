using System;
using System.Collections;
using LL.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LL.Bootstrap
{
    [DisallowMultipleComponent]
    internal sealed class BootstrapController : MonoBehaviour
    {
        [SerializeField] private ProgressBar _progressBar;
        [SerializeField] private float _minimumDisplaySeconds = 0.75f;

        private const string TargetSceneName = "Main";
        private const float ReadyProgress = 0.9f;

        private IEnumerator Start()
        {
            _progressBar.SetProgress(0f);
            yield return null;

            var startedAt = Time.realtimeSinceStartup;
            var operation = SceneManager.LoadSceneAsync(TargetSceneName, LoadSceneMode.Single);

            if (operation == null)
                throw new InvalidOperationException($"Failed to start loading scene '{TargetSceneName}'.");

            operation.allowSceneActivation = false;

            while (operation.isDone is false)
            {
                var loadProgress = Mathf.Clamp01(operation.progress / ReadyProgress);
                var timeProgress = _minimumDisplaySeconds <= 0f
                    ? 1f
                    : Mathf.Clamp01((Time.realtimeSinceStartup - startedAt) / _minimumDisplaySeconds);

                _progressBar.SetProgress(Mathf.Min(loadProgress, timeProgress));

                if (loadProgress >= 1f && timeProgress >= 1f)
                {
                    _progressBar.SetProgress(1f);
                    yield return null;
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}