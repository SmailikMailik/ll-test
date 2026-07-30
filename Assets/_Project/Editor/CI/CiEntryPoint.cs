using UnityEngine;

namespace LLEditor.CI
{
    public static class CiEntryPoint
    {
        public static void Validate()
        {
            ProjectCiValidation.Run();
            Debug.Log("CI validation completed successfully.");
        }

        public static void Build()
        {
            ProjectCiValidation.Run();
            ProjectBuildPipeline.Build();
        }
    }
}