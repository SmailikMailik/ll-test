using LL.Saving;
using LL.User.Persistence;
using LLEditor.Configuration;
using UnityEditor;
using UnityEngine;

namespace LLEditor.User
{
    internal static class UserSaveMenu
    {
        private const string MenuPath = ConfigurationAssetMenu.RootPath + "User/Delete Saved Data";

        [MenuItem(MenuPath)]
        private static void DeleteSavedData()
        {
            const string saveKey = UserInitialDataLoader.SaveKey;
            ISaveService saveService = new JsonFileSaveService();

            if (saveService.Exists(saveKey) is false)
            {
                Debug.Log("Saved user data was not found.");
                return;
            }

            var confirmed = EditorUtility.DisplayDialog(
                "Delete Saved Data",
                "Delete all saved user data? This action cannot be undone.",
                "Delete",
                "Cancel");

            if (confirmed is false)
                return;

            if (saveService.TryDelete(saveKey))
                Debug.Log("Saved user data was deleted.");
        }

        [MenuItem(MenuPath, true)]
        private static bool CanDeleteSavedData() => EditorApplication.isPlayingOrWillChangePlaymode is false;
    }
}