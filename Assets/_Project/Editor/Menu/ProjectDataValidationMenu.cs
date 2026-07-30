using LLEditor.Validation;
using UnityEditor;

namespace LLEditor.Menu
{
    internal static class ProjectDataValidationMenu
    {
        [MenuItem(LastLevelMenu.ValidateProjectDataPath, false, LastLevelMenu.ValidateProjectDataPriority)]
        private static void ValidateProjectData()
        {
            var result = ProjectDataValidationRunner.Run(out var assetCount);
            ProjectDataValidationRunner.Report(result, assetCount);
        }
    }
}