using UnityEditor;

namespace LLEditor.Menu
{
    internal static class SceneMenu
    {
        [MenuItem(LastLevelMenu.Scenes.Path + "Open Bootstrap Scene", false, LastLevelMenu.Scenes.Priority)]
        private static void OpenBootstrapScene() => ProjectSceneOpener.Open(ProjectScenePaths.Bootstrap);

        [MenuItem(LastLevelMenu.Scenes.Path + "Open Bootstrap Scene", true, LastLevelMenu.Scenes.Priority)]
        private static bool CanOpenBootstrapScene() => ProjectSceneOpener.CanOpen;

        [MenuItem(LastLevelMenu.Scenes.Path + "Open Main Scene", false, LastLevelMenu.Scenes.Priority + 1)]
        private static void OpenMainScene() => ProjectSceneOpener.Open(ProjectScenePaths.Main);

        [MenuItem(LastLevelMenu.Scenes.Path + "Open Main Scene", true, LastLevelMenu.Scenes.Priority + 1)]
        private static bool CanOpenMainScene() => ProjectSceneOpener.CanOpen;
    }
}