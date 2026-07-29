using System;
using System.Linq;
using LL.Infrastructure.Validation;
using LL.Validation;
using LLEditor.Validation.References;
using LLEditor.Validation.Sources;
using UnityEditor;
using UnityEngine;

namespace LLEditor.Validation
{
    internal static class ProjectDataValidationRunner
    {
        private const string ProjectDataPath = "Assets/_Project/ScriptableObjects";
        private const string ProjectDataRoot = "ProjectData";

        internal static ValidationResult Run(out int assetCount)
        {
            var assets = AssetDatabase
                .FindAssets(string.Empty, new[] { ProjectDataPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => AssetDatabase.LoadAssetAtPath<ScriptableObject>(path))
                .Where(asset => asset is IValidationSource)
                .ToArray();

            var result = new ValidationResult();
            var context = new ValidationContext(result);
            var sources = new ProjectDataSources(assets);
            var sourceValidator = new ProjectDataSourceValidator();

            sourceValidator.Validate(sources, context.At(ProjectDataRoot));

            foreach (var asset in assets)
            {
                var source = (IValidationSource)asset;
                var assetPath = AssetDatabase.GetAssetPath(asset);
                source.Validate(context.At(assetPath));
            }

            var referenceValidator = new ProjectDataReferenceValidator();
            referenceValidator.Validate(sources, context);

            assetCount = assets.Length;
            return result;
        }

        internal static void Report(ValidationResult result, int assetCount)
        {
            new UnityConsoleValidationReporter().Report(result);

            if (result.IsValid)
            {
                Debug.Log(
                    $"Project data validation succeeded. Checked {assetCount} assets with " +
                    $"{result.WarningCount} warnings and {result.InfoCount} informational issues.");
                return;
            }

            Debug.LogError(
                $"Project data validation failed with {result.ErrorCount} errors and " +
                $"{result.WarningCount} warnings across {assetCount} assets. " +
                $"Informational issues: {result.InfoCount}.");
        }
    }
}