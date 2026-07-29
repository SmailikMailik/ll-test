using LLEditor.Menu;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LLEditor.Scenes
{
    internal static class MainSceneMenu
    {
        private const string MainScenePath = "Assets/_Project/Scenes/Main.unity";
        private const string WelcomePreferenceKey = "LL.Editor.MainSceneWelcomeShown";

        [InitializeOnLoadMethod]
        private static void ScheduleWelcome()
        {
            if (EditorPrefs.GetBool(WelcomePreferenceKey))
                return;

            EditorApplication.delayCall += OnEditorReady;
        }

        [MenuItem(LLMenu.OpenMainScenePath, false, LLMenu.OpenMainScenePriority)]
        private static void OpenMainScene() => TryOpenMainScene();

        [MenuItem(LLMenu.OpenMainScenePath, true, LLMenu.OpenMainScenePriority)]
        private static bool CanOpenMainScene() => EditorApplication.isPlayingOrWillChangePlaymode is false;

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

            var openMainScene = EditorUtility.DisplayDialog(
                "Welcome to LL-Test Project",
                "The currently open scene is empty. Would you like to open the Main scene?",
                "Open Main",
                "Not Now");

            if (openMainScene)
                TryOpenMainScene();
        }

        private static bool TryOpenMainScene()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(MainScenePath) == null)
            {
                Debug.LogError($"Main scene was not found at '{MainScenePath}'.");
                return false;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() is false)
                return false;

            EditorSceneManager.OpenScene(MainScenePath);
            return true;
        }
    }
}