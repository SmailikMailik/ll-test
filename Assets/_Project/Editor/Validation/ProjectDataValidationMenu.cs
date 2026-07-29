using LLEditor.Menu;
using UnityEditor;

namespace LLEditor.Validation
{
    internal static class ProjectDataValidationMenu
    {
        [MenuItem(LLMenu.ValidateProjectDataPath, false, LLMenu.ValidateProjectDataPriority)]
        private static void ValidateProjectData()
        {
            var result = ProjectDataValidationRunner.Run(out var assetCount);
            ProjectDataValidationRunner.Report(result, assetCount);
        }
    }
}