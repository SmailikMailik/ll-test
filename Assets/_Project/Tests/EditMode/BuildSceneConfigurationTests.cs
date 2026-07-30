using NUnit.Framework;
using UnityEditor;

namespace LL.Tests.EditMode
{
    internal sealed class BuildSceneConfigurationTests
    {
        private const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";
        private const string MainScenePath = "Assets/_Project/Scenes/Main.unity";

        [Test]
        public void BootstrapAndMainScenesAreEnabledInOrder()
        {
            var scenes = EditorBuildSettings.scenes;

            Assert.That(scenes.Length, Is.GreaterThanOrEqualTo(2));
            Assert.That(scenes[0].enabled, Is.True);
            Assert.That(scenes[0].path, Is.EqualTo(BootstrapScenePath));
            Assert.That(scenes[1].enabled, Is.True);
            Assert.That(scenes[1].path, Is.EqualTo(MainScenePath));
        }

        [TestCase(BootstrapScenePath)]
        [TestCase(MainScenePath)]
        public void RequiredSceneExists(string scenePath)
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath), Is.Not.Null);
        }
    }
}