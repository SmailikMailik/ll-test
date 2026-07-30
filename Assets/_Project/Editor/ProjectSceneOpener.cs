using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LLEditor
{
    internal static class ProjectSceneOpener
    {
        internal static bool CanOpen => EditorApplication.isPlayingOrWillChangePlaymode is false;

        internal static void Open(string scenePath)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
                Debug.LogError($"Scene was not found at '{scenePath}'.");
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() is false)
                return;

            EditorSceneManager.OpenScene(scenePath);
        }
    }
}