using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace LL.Tests.PlayMode
{
    internal sealed class BootstrapSmokeTests
    {
        private const string BootstrapSceneName = "Bootstrap";
        private const string MainSceneName = "Main";
        private const float TimeoutSeconds = 10f;

        [UnityTest]
        public IEnumerator BootstrapLoadsMainScene()
        {
            var loadOperation = SceneManager.LoadSceneAsync(BootstrapSceneName, LoadSceneMode.Single);
            Assert.That(loadOperation, Is.Not.Null);

            while (loadOperation.isDone is false)
                yield return null;

            var deadline = Time.realtimeSinceStartup + TimeoutSeconds;

            while (SceneManager.GetActiveScene().name != MainSceneName &&
                   Time.realtimeSinceStartup < deadline)
            {
                yield return null;
            }

            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(MainSceneName));
        }
    }
}