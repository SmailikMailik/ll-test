using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LLEditor.Menu
{
    internal static class SceneMenu
    {
        private const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";
        private const string MainScenePath = "Assets/_Project/Scenes/Main.unity";
        private const string WelcomePreferenceKey = "LL.Editor.BootstrapSceneWelcomeShown";
        private const string BootstrapPlayOverrideKey = "LL.Editor.BootstrapPlayOverride";
        private const string PreviousPlayModeStartSceneKey = "LL.Editor.PreviousPlayModeStartScene";
        private const string NativePlayMigrationKey = "LL.Editor.NativePlayRestored.v1";

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (EditorApplication.isPlayingOrWillChangePlaymode is false)
            {
                MigrateLegacyPlayModeStartScene();
                RestorePlayModeStartScene();
            }

            if (EditorPrefs.GetBool(WelcomePreferenceKey))
                return;

            EditorApplication.delayCall += OnEditorReady;
        }

        [MenuItem(LastLevelMenu.ScenesPath + "Open Bootstrap Scene", false, LastLevelMenu.ScenePriority)]
        private static void OpenBootstrapScene() => TryOpenScene(BootstrapScenePath, "Bootstrap");

        [MenuItem(LastLevelMenu.ScenesPath + "Open Bootstrap Scene", true, LastLevelMenu.ScenePriority)]
        private static bool CanOpenBootstrapScene() => CanOpenScene();

        [MenuItem(LastLevelMenu.ScenesPath + "Open Main Scene", false, LastLevelMenu.ScenePriority + 1)]
        private static void OpenMainScene() => TryOpenScene(MainScenePath, "Main");

        [MenuItem(LastLevelMenu.ScenesPath + "Open Main Scene", true, LastLevelMenu.ScenePriority + 1)]
        private static bool CanOpenMainScene() => CanOpenScene();

        internal static void PlayFromBootstrap()
        {
            if (CanStartPlayMode() is false)
                return;

            var bootstrapScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScenePath);

            if (bootstrapScene == null)
            {
                Debug.LogError($"Bootstrap scene was not found at '{BootstrapScenePath}'.");
                return;
            }

            var previousStartScenePath = AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene);
            SessionState.SetString(PreviousPlayModeStartSceneKey, previousStartScenePath);
            SessionState.SetBool(BootstrapPlayOverrideKey, true);
            EditorSceneManager.playModeStartScene = bootstrapScene;
            EditorApplication.isPlaying = true;
        }

        private static bool CanOpenScene() => EditorApplication.isPlayingOrWillChangePlaymode is false;

        internal static bool CanStartPlayMode() => EditorApplication.isPlayingOrWillChangePlaymode is false;

        private static void OnEditorReady()
        {
            EditorApplication.delayCall -= OnEditorReady;

            if (EditorPrefs.GetBool(WelcomePreferenceKey))
                return;

            EditorPrefs.SetBool(WelcomePreferenceKey, true);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var activeScene = SceneManager.GetActiveScene();

            if (string.IsNullOrEmpty(activeScene.path) is false)
                return;

            var openBootstrapScene = EditorUtility.DisplayDialog(
                "Welcome to Last Level",
                "The currently open scene is empty. Would you like to open the Bootstrap scene?",
                "Open Bootstrap",
                "Not Now");

            if (openBootstrapScene)
                TryOpenScene(BootstrapScenePath, "Bootstrap");
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                MigrateLegacyPlayModeStartScene();
                RestorePlayModeStartScene();
            }
        }

        private static void MigrateLegacyPlayModeStartScene()
        {
            if (EditorPrefs.GetBool(NativePlayMigrationKey))
                return;

            EditorPrefs.SetBool(NativePlayMigrationKey, true);

            var startScenePath = AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene);

            if (startScenePath == BootstrapScenePath)
                EditorSceneManager.playModeStartScene = null;
        }

        private static void RestorePlayModeStartScene()
        {
            if (SessionState.GetBool(BootstrapPlayOverrideKey, false) is false)
                return;

            SessionState.SetBool(BootstrapPlayOverrideKey, false);
            var previousStartScenePath = SessionState.GetString(PreviousPlayModeStartSceneKey, string.Empty);
            SessionState.SetString(PreviousPlayModeStartSceneKey, string.Empty);
            EditorSceneManager.playModeStartScene =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(previousStartScenePath);
        }

        private static bool TryOpenScene(string scenePath, string sceneName)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
                Debug.LogError($"{sceneName} scene was not found at '{scenePath}'.");
                return false;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() is false)
                return false;

            EditorSceneManager.OpenScene(scenePath);
            return true;
        }
    }
}