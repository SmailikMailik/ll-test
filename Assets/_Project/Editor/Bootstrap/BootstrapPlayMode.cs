using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LLEditor.Bootstrap
{
    [InitializeOnLoad]
    internal static class BootstrapPlayMode
    {
        private const string PreviousStartSceneKey = "LL.Editor.BootstrapPlay.PreviousStartScene";
        private const string NoStartSceneValue = "<none>";

        internal static bool CanStart => EditorApplication.isPlayingOrWillChangePlaymode is false;

        static BootstrapPlayMode()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (CanStart)
                RestorePreviousStartScene();
        }

        internal static void Play()
        {
            if (CanStart is false)
                return;

            var bootstrapScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ProjectScenePaths.Bootstrap);

            if (bootstrapScene == null)
            {
                Debug.LogError($"Bootstrap scene was not found at '{ProjectScenePaths.Bootstrap}'.");
                return;
            }

            var previousScenePath = AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene);
            var previousSceneValue = string.IsNullOrEmpty(previousScenePath)
                ? NoStartSceneValue
                : previousScenePath;
            SessionState.SetString(PreviousStartSceneKey, previousSceneValue);
            EditorSceneManager.playModeStartScene = bootstrapScene;
            EditorApplication.isPlaying = true;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
                RestorePreviousStartScene();
        }

        private static void RestorePreviousStartScene()
        {
            var previousSceneValue = SessionState.GetString(PreviousStartSceneKey, string.Empty);

            if (string.IsNullOrEmpty(previousSceneValue))
                return;

            SessionState.SetString(PreviousStartSceneKey, string.Empty);
            EditorSceneManager.playModeStartScene = previousSceneValue == NoStartSceneValue
                ? null
                : AssetDatabase.LoadAssetAtPath<SceneAsset>(previousSceneValue);
        }
    }
}