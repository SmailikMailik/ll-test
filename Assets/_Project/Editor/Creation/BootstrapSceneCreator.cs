using System;
using System.Collections.Generic;
using System.IO;
using LL.Bootstrap;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LLEditor.Creation
{
    internal static class BootstrapSceneCreator
    {
        internal const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";
        internal const string MainScenePath = "Assets/_Project/Scenes/Main.unity";

        private const string MenuPath = "Tools/LL/Project/Create Bootstrap Scene";

        [MenuItem(MenuPath)]
        private static void CreateFromMenu()
        {
            if (File.Exists(BootstrapScenePath) &&
                EditorUtility.DisplayDialog(
                    "Create Bootstrap Scene",
                    "Replace the existing Bootstrap scene?",
                    "Replace",
                    "Cancel") is false)
            {
                return;
            }

            CreateOrReplace();
        }

        public static void CreateOrReplace()
        {
            var previousActiveScene = SceneManager.GetActiveScene();
            var bootstrapScene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Additive);

            try
            {
                SceneManager.SetActiveScene(bootstrapScene);
                CreateSceneContent();

                if (EditorSceneManager.SaveScene(bootstrapScene, BootstrapScenePath) is false)
                    throw new InvalidOperationException("Failed to save the Bootstrap scene.");

                UpdateBuildScenes();
            }
            finally
            {
                if (bootstrapScene.IsValid())
                    EditorSceneManager.CloseScene(bootstrapScene, true);

                if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
                    SceneManager.SetActiveScene(previousActiveScene);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Bootstrap scene created at '{BootstrapScenePath}'.");
        }

        private static void CreateSceneContent()
        {
            var bootstrap = new GameObject("Bootstrap");
            var controller = bootstrap.AddComponent<BootstrapController>();
            var view = BootstrapLoadingView.Create();

            var serializedController = new SerializedObject(controller);
            serializedController.FindProperty("_progressFill").objectReferenceValue = view.ProgressFill;
            serializedController.FindProperty("_loadingLabel").objectReferenceValue = view.LoadingLabel;
            serializedController.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void UpdateBuildScenes()
        {
            if (File.Exists(MainScenePath) is false)
                throw new FileNotFoundException("Main scene was not found.", MainScenePath);

            var scenes = new List<EditorBuildSettingsScene>
            {
                new(BootstrapScenePath, true),
                new(MainScenePath, true)
            };

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == BootstrapScenePath || scene.path == MainScenePath)
                    continue;

                scenes.Add(scene);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}