using System;
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
        private const string PrefabsRoot = "Assets/_Project/Prefabs";

        [Test]
        public void StateEffect_IsSerializableManagedContract()
        {
            Assert.That(typeof(StateEffect).GetCustomAttribute<SerializableAttribute>(), Is.Not.Null);
            Assert.That(typeof(Component).IsAssignableFrom(typeof(StateEffect)), Is.False);
        }

        [Test]
        public void StateRenderers_InPrefabsContainValidEffects()
        {
            ForEachPrefab((prefab, path) =>
            {
                foreach (var renderer in prefab.GetComponentsInChildren<StateRenderer>(true))
                {
                    var ownerPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(renderer);

                    if (!string.IsNullOrEmpty(ownerPath) &&
                        !string.Equals(ownerPath, path, StringComparison.Ordinal))
                        continue;

                    AssertRenderer(renderer, path);
                }
            });
        }

        [Test]
        public void StateOwners_InPrefabsReferenceRenderers()
        {
            ForEachPrefab((prefab, path) =>
            {
                foreach (var button in prefab.GetComponentsInChildren<InteractiveButton>(true))
                    AssertStateRenderer(button, path);

                foreach (var cardView in prefab.GetComponentsInChildren<UpgradeCardView>(true))
                    AssertStateRenderer(cardView, path);
            });
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
            var states = effect.StateValues;

            Assert.That(effect.IsTargetValid(renderer), Is.True, $"{context}, {effectType.Name} target");
            Assert.That(states, Is.Not.Null, $"{context}, {effectType.Name} states");
            Assert.That(
                states.Count,
                Is.EqualTo(Enum.GetValues(renderer.StateType).Length),
                $"{context}, {effectType.Name} states");

            foreach (var state in Enum.GetValues(renderer.StateType))
            {
                var stateValue = Convert.ToInt32(state);
                var matchingStates = 0;

                foreach (var value in states)
                {
                    if (value is not null && value.State == stateValue)
                        matchingStates++;
                }

                Assert.That(
                    matchingStates,
                    Is.EqualTo(1),
                    $"{context}, {effectType.Name}, state {state}");
            }
        }

        private static void ForEachPrefab(Action<GameObject, string> assertPrefab)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:Prefab", new[] { PrefabsRoot }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = PrefabUtility.LoadPrefabContents(path);

                try
                {
                    assertPrefab(prefab, path);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(prefab);
                }
            }
        }
    }
}