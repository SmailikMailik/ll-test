using UnityEditor;

namespace LLEditor.Menu
{
    internal static class SceneMenu
    {
        [MenuItem(LastLevelMenu.ScenesPath + "Open Bootstrap Scene", false, LastLevelMenu.ScenePriority)]
        private static void OpenBootstrapScene() => ProjectSceneOpener.Open(ProjectScenePaths.Bootstrap);

        [MenuItem(LastLevelMenu.ScenesPath + "Open Bootstrap Scene", true, LastLevelMenu.ScenePriority)]
        private static bool CanOpenBootstrapScene() => ProjectSceneOpener.CanOpen;

        [MenuItem(LastLevelMenu.ScenesPath + "Open Main Scene", false, LastLevelMenu.ScenePriority + 1)]
        private static void OpenMainScene() => ProjectSceneOpener.Open(ProjectScenePaths.Main);

        [MenuItem(LastLevelMenu.ScenesPath + "Open Main Scene", true, LastLevelMenu.ScenePriority + 1)]
        private static bool CanOpenMainScene() => ProjectSceneOpener.CanOpen;
    }
}