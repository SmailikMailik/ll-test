using System;
using System.IO;
using System.Linq;
using LLEditor.Validation;
using UnityEditor;
using UnityEditor.Build;

namespace LLEditor.CI
{
    internal static class ProjectCiValidation
    {
        internal static void Run()
        {
            ValidateBuildScenes();

            var result = ProjectDataValidationRunner.Run(out var assetCount);
            ProjectDataValidationRunner.Report(result, assetCount);

            if (result.IsValid is false)
                throw new BuildFailedException($"Project data validation failed with {result.ErrorCount} errors.");
        }

        internal static string[] GetEnabledBuildScenes()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
        }

        private static void ValidateBuildScenes()
        {
            var scenes = GetEnabledBuildScenes();

            if (scenes.Length < 2)
                throw new BuildFailedException("At least Bootstrap and Main scenes must be enabled.");

            if (scenes[0] != ProjectScenePaths.Bootstrap)
                throw new BuildFailedException($"Bootstrap scene must be the first enabled build scene: '{ProjectScenePaths.Bootstrap}'.");

            if (scenes.Contains(ProjectScenePaths.Main, StringComparer.Ordinal) is false)
                throw new BuildFailedException($"Main scene is not enabled: '{ProjectScenePaths.Main}'.");

            foreach (var scenePath in scenes)
            {
                if (File.Exists(scenePath) is false)
                    throw new BuildFailedException($"Build scene does not exist: '{scenePath}'.");
            }
        }
    }
}