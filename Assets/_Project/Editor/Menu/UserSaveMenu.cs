using LL.Composition.Factories;
using LL.Infrastructure.Saving.Serialization;
using LL.Infrastructure.Saving.Storage;
using UnityEditor;
using UnityEngine;

namespace LLEditor.Menu
{
    internal static class UserSaveMenu
    {
        [MenuItem(LastLevelMenu.Commands.DeleteSavedUserDataPath, false, LastLevelMenu.Commands.DeleteSavedUserDataPriority)]
        private static void DeleteSavedUserData()
        {
            var repository = UserSaveRepositoryFactory.CreateSerialized(
                new JsonSaveSerializer(),
                new FileSaveStorage());

            if (repository.Exists() is false)
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

            if (repository.Delete())
                Debug.Log("Saved user data was deleted.");
        }

        [MenuItem(LastLevelMenu.Commands.DeleteSavedUserDataPath, true, LastLevelMenu.Commands.DeleteSavedUserDataPriority)]
        private static bool CanDeleteSavedUserData() => EditorApplication.isPlayingOrWillChangePlaymode is false;
    }
}