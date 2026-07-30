using System.Collections;
using LL.UI;
using UnityEngine;

namespace LL.Bootstrap
{
    [DisallowMultipleComponent]
    internal sealed class BootstrapController : MonoBehaviour
    {
        [SerializeField] private ProgressBar _progressBar;
        [SerializeField, Min(0f)] private float _minDisplaySeconds = 0.75f;

        private const string TargetSceneName = "Main";

        private IEnumerator Start()
        {
            _progressBar.SetProgress(0f);
            yield return null;

            var loadingOperation = new BootstrapLoadingOperation(
                TargetSceneName,
                _minDisplaySeconds);

            while (loadingOperation.IsReady is false)
            {
                loadingOperation.EnsureSucceeded();
                _progressBar.SetProgress(loadingOperation.Progress);
                yield return null;
            }

            _progressBar.SetProgress(1f);
            yield return null;
            loadingOperation.ActivateScene();
        }
    }
}