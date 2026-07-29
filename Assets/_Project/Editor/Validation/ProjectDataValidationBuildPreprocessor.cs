using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace LLEditor.Validation
{
    internal sealed class ProjectDataValidationBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport _)
        {
            var result = ProjectDataValidationRunner.Run(out var assetCount);
            ProjectDataValidationRunner.Report(result, assetCount);

            if (result.IsValid is false)
            {
                throw new BuildFailedException(
                    $"Project data validation failed with {result.ErrorCount} errors.");
            }
        }
    }
}