using System;
using System.Collections;
using System.Reflection;
using LL.UI.Controls;
using LL.UI.VisualStates.Effects;
using LL.UI.VisualStates.Sources;
using LL.UI.Windows.Upgrade.Cards;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LL.Tests.EditMode.UI.VisualStates
{
    internal sealed class VisualStatePrefabTests
    {
        [Test]
        public void VisualStateEffect_IsSerializableManagedContract()
        {
            Assert.That(typeof(VisualStateEffect).GetCustomAttribute<SerializableAttribute>(), Is.Not.Null);
            Assert.That(typeof(Component).IsAssignableFrom(typeof(VisualStateEffect)), Is.False);
        }

        [Test]
        public void VisualStateSources_InPrefabsContainValidEffects()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Project/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = PrefabUtility.LoadPrefabContents(path);

                try
                {
                    foreach (var source in prefab.GetComponentsInChildren<VisualStateSource>(true))
                    {
                        var ownerPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(source);

                        if (!string.IsNullOrEmpty(ownerPath) &&
                            !string.Equals(ownerPath, path, StringComparison.Ordinal))
                            continue;

                        AssertSource(source, path);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
        }

        [Test]
        public void VisualStateControllers_InPrefabsReferenceSources()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Project/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = PrefabUtility.LoadPrefabContents(path);

                try
                {
                    foreach (var button in prefab.GetComponentsInChildren<InteractiveButton>(true))
                        AssertStateSource(button, path);

                    foreach (var cardView in prefab.GetComponentsInChildren<UpgradeCardView>(true))
                        AssertStateSource(cardView, path);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
        }

        private static void AssertStateSource(Component controller, string path)
        {
            var context = $"{path} ({controller.name}, {controller.GetType().Name})";
            var serializedController = new SerializedObject(controller);
            var stateSource = serializedController.FindProperty("_stateSource");

            Assert.That(stateSource, Is.Not.Null, context);
            Assert.That(stateSource.objectReferenceValue, Is.Not.Null, context);
        }

        private static void AssertSource(VisualStateSource source, string path)
        {
            var context = $"{path} ({source.name})";
            var serializedSource = new SerializedObject(source);
            var effects = serializedSource.FindProperty("_effects");

            Assert.That(effects, Is.Not.Null, context);
            Assert.That(effects.arraySize, Is.GreaterThan(0), context);

            for (var index = 0; index < effects.arraySize; index++)
            {
                var effect = effects.GetArrayElementAtIndex(index).managedReferenceValue as VisualStateEffect;
                Assert.That(effect, Is.Not.Null, $"{context}, effect {index}");
                AssertEffect(effect, source, context);
            }
        }

        private static void AssertEffect(
            VisualStateEffect effect,
            VisualStateSource source,
            string context)
        {
            var effectType = effect.GetType();
            var targetField = effectType.GetField("_target", BindingFlags.Instance | BindingFlags.NonPublic);
            var statesField = effectType.GetField("_states", BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(targetField, Is.Not.Null, $"{context}, {effectType.Name}");
            Assert.That(targetField.GetValue(effect), Is.Not.Null, $"{context}, {effectType.Name} target");
            Assert.That(statesField, Is.Not.Null, $"{context}, {effectType.Name}");

            var states = statesField.GetValue(effect) as IList;
            Assert.That(states, Is.Not.Null, $"{context}, {effectType.Name} states");
            Assert.That(
                states.Count,
                Is.EqualTo(Enum.GetValues(source.StateType).Length),
                $"{context}, {effectType.Name} states");
        }
    }
}