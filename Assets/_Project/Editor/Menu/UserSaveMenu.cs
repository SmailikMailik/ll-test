using LL.Composition.Factories;
using LL.User.Persistence;
using UnityEditor;
using UnityEngine;

namespace LLEditor.Menu
{
    internal static class UserSaveMenu
    {
        [MenuItem(LastLevelMenu.DeleteSavedUserDataPath, false, LastLevelMenu.DeleteSavedUserDataPriority)]
        private static void DeleteSavedUserData()
        {
            const string saveKey = UserSnapshotLoader.SaveKey;
            var saveService = UserSaveServiceFactory.CreateJsonFile();

            if (saveService.Exists(saveKey) is false)
            {
                Debug.Log("Saved user data was not found.");
                return;
            }

            var confirmed = EditorUtility.DisplayDialog(
                "Delete Saved User Data",
                "Delete all saved user data? This action cannot be undone.",
                "Delete",
                "Cancel");

            if (confirmed is false)
                return;

            if (saveService.TryDelete(saveKey))
                Debug.Log("Saved user data was deleted.");
        }

        [MenuItem(LastLevelMenu.DeleteSavedUserDataPath, true, LastLevelMenu.DeleteSavedUserDataPriority)]
        private static bool CanDeleteSavedUserData() => EditorApplication.isPlayingOrWillChangePlaymode is false;
    }
}