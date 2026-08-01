using LLEditor;
using NUnit.Framework;
using UnityEditor;

namespace LL.Tests.EditMode.Project
{
    internal sealed class BuildSceneConfigurationTests
    {
        [Test]
        public void BootstrapAndMainScenesAreEnabledInOrder()
        {
            var scenes = EditorBuildSettings.scenes;

            Assert.That(scenes.Length, Is.GreaterThanOrEqualTo(2));
            Assert.That(scenes[0].enabled, Is.True);
            Assert.That(scenes[0].path, Is.EqualTo(ProjectScenePaths.Bootstrap));
            Assert.That(scenes[1].enabled, Is.True);
            Assert.That(scenes[1].path, Is.EqualTo(ProjectScenePaths.Main));
        }

        [TestCase(ProjectScenePaths.Bootstrap)]
        [TestCase(ProjectScenePaths.Main)]
        public void RequiredSceneExists(string scenePath)
        {
            Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath), Is.Not.Null);
        }
    }
}