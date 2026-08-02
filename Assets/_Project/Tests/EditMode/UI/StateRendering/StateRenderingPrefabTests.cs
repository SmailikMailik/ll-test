using System;
using System.Collections;
using System.Reflection;
using LL.UI.Controls;
using LL.UI.StateRendering.Effects;
using LL.UI.StateRendering.Renderers;
using LL.UI.Windows.Upgrade.Cards;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LL.Tests.EditMode.UI.StateRendering
{
    internal sealed class StateRenderingPrefabTests
    {
        [Test]
        public void StateEffect_IsSerializableManagedContract()
        {
            Assert.That(typeof(StateEffect).GetCustomAttribute<SerializableAttribute>(), Is.Not.Null);
            Assert.That(typeof(Component).IsAssignableFrom(typeof(StateEffect)), Is.False);
        }

        [Test]
        public void StateRenderers_InPrefabsContainValidEffects()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Project/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = PrefabUtility.LoadPrefabContents(path);

                try
                {
                    foreach (var renderer in prefab.GetComponentsInChildren<StateRenderer>(true))
                    {
                        var ownerPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(renderer);

                        if (!string.IsNullOrEmpty(ownerPath) &&
                            !string.Equals(ownerPath, path, StringComparison.Ordinal))
                            continue;

                        AssertRenderer(renderer, path);
                    }
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
        }

        [Test]
        public void StateOwners_InPrefabsReferenceRenderers()
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/_Project/Prefabs" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = PrefabUtility.LoadPrefabContents(path);

                try
                {
                    foreach (var button in prefab.GetComponentsInChildren<InteractiveButton>(true))
                        AssertStateRenderer(button, path);

                    foreach (var cardView in prefab.GetComponentsInChildren<UpgradeCardView>(true))
                        AssertStateRenderer(cardView, path);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
        }

        private static void AssertStateRenderer(Component owner, string path)
        {
            var context = $"{path} ({owner.name}, {owner.GetType().Name})";
            var serializedOwner = new SerializedObject(owner);
            var stateRenderer = serializedOwner.FindProperty("_stateRenderer");

            Assert.That(stateRenderer, Is.Not.Null, context);
            Assert.That(stateRenderer.objectReferenceValue, Is.Not.Null, context);
        }

        private static void AssertRenderer(StateRenderer renderer, string path)
        {
            var context = $"{path} ({renderer.name})";
            var serializedRenderer = new SerializedObject(renderer);
            var effects = serializedRenderer.FindProperty("_effects");

            Assert.That(effects, Is.Not.Null, context);
            Assert.That(effects.arraySize, Is.GreaterThan(0), context);

            for (var index = 0; index < effects.arraySize; index++)
            {
                var effect = effects.GetArrayElementAtIndex(index).managedReferenceValue as StateEffect;
                Assert.That(effect, Is.Not.Null, $"{context}, effect {index}");
                AssertEffect(effect, renderer, context);
            }
        }

        private static void AssertEffect(
            StateEffect effect,
            StateRenderer renderer,
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
                Is.EqualTo(Enum.GetValues(renderer.StateType).Length),
                $"{context}, {effectType.Name} states");
        }
    }
}