using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LLEditor.CI
{
    internal static class ProjectBuildPipeline
    {
        private const string OutputEnvironmentVariable = "LL_BUILD_OUTPUT";
        private const string VersionEnvironmentVariable = "LL_BUILD_VERSION";
        private const string BuildNumberEnvironmentVariable = "LL_BUILD_NUMBER";

        internal static void Build()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var outputPath = GetOutputPath(target);
            var outputDirectory = GetOutputDirectory(target, outputPath);
            Directory.CreateDirectory(outputDirectory);

            var previousVersion = PlayerSettings.bundleVersion;
            var previousAndroidVersionCode = PlayerSettings.Android.bundleVersionCode;

            try
            {
                ApplyVersion();

                var options = new BuildPlayerOptions
                {
                    scenes = ProjectCiValidation.GetEnabledBuildScenes(),
                    locationPathName = outputPath,
                    target = target,
                    options = BuildOptions.CleanBuildCache
                };
                var report = BuildPipeline.BuildPlayer(options);

                if (report.summary.result != BuildResult.Succeeded)
                {
                    throw new BuildFailedException($"Player build failed with result {report.summary.result}.");
                }

                Debug.Log(
                    $"Player build completed: target={target}, " +
                    $"size={report.summary.totalSize}, output='{outputPath}'.");
            }
            finally
            {
                PlayerSettings.bundleVersion = previousVersion;
                PlayerSettings.Android.bundleVersionCode = previousAndroidVersionCode;
            }
        }

        private static string GetOutputPath(BuildTarget target)
        {
            var configuredPath = Environment.GetEnvironmentVariable(OutputEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(configuredPath) is false)
                return Path.GetFullPath(configuredPath);

            return target switch
            {
                BuildTarget.StandaloneWindows64 => Path.GetFullPath("Builds/Windows/LL-Test.exe"),
                BuildTarget.Android => Path.GetFullPath("Builds/Android/LL-Test.apk"),
                BuildTarget.WebGL => Path.GetFullPath("Builds/WebGL"),
                _ => throw new BuildFailedException(
                    $"Build output is not configured for target {target}. " +
                    $"Set {OutputEnvironmentVariable}.")
            };
        }

        private static string GetOutputDirectory(BuildTarget target, string outputPath)
        {
            if (target == BuildTarget.WebGL)
                return outputPath;

            return Path.GetDirectoryName(outputPath) ?? throw new BuildFailedException($"Build output has no directory: '{outputPath}'.");
        }

        private static void ApplyVersion()
        {
            var version = Environment.GetEnvironmentVariable(VersionEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(version) is false)
                PlayerSettings.bundleVersion = version;

            var buildNumber = Environment.GetEnvironmentVariable(BuildNumberEnvironmentVariable);

            if (string.IsNullOrWhiteSpace(buildNumber))
                return;

            if (int.TryParse(buildNumber, out var parsedBuildNumber) is false || parsedBuildNumber <= 0)
                throw new BuildFailedException($"{BuildNumberEnvironmentVariable} must be a positive integer.");

            PlayerSettings.Android.bundleVersionCode = parsedBuildNumber;
        }
    }
}