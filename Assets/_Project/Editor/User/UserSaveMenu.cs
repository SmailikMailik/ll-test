using LL.Composition.Persistence;
using LL.User.Persistence;
using LLEditor.Menu;
using UnityEditor;
using UnityEngine;

namespace LLEditor.User
{
    internal static class UserSaveMenu
    {
        [MenuItem(LLMenu.DeleteSavedUserDataPath, false, LLMenu.DeleteSavedUserDataPriority)]
        private static void DeleteSavedUserData()
        {
            const string saveKey = UserSnapshotLoader.SaveKey;
            var saveService = PersistenceComposition.CreateDefaultSaveService();

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

        [MenuItem(LLMenu.DeleteSavedUserDataPath, true, LLMenu.DeleteSavedUserDataPriority)]
        private static bool CanDeleteSavedUserData() => EditorApplication.isPlayingOrWillChangePlaymode is false;
    }
}