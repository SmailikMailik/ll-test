using System.Linq;
using LL.Infrastructure.Validation;
using LL.Validation;
using LLEditor.Menu;
using LLEditor.Validation.References;
using LLEditor.Validation.Sources;
using UnityEditor;
using UnityEngine;

namespace LLEditor.Validation
{
    internal static class ProjectDataValidationMenu
    {
        private const string ProjectDataPath = "Assets/_Project/ScriptableObjects";

        [MenuItem(LLMenu.ValidateProjectDataPath, false, LLMenu.ValidateProjectDataPriority)]
        private static void ValidateProjectData()
        {
            var assets = AssetDatabase
                .FindAssets(string.Empty, new[] { ProjectDataPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .OrderBy(path => path)
                .Select(path => AssetDatabase.LoadAssetAtPath<ScriptableObject>(path))
                .Where(asset => asset is IValidationSource)
                .ToArray();

            var result = new ValidationResult();
            var context = new ValidationContext(result);
            var sources = new ProjectDataSources(assets);
            var sourceValidator = new ProjectDataSourceValidator();

            sourceValidator.Validate(sources, context.At("ProjectData"));

            foreach (var asset in assets)
            {
                var source = (IValidationSource)asset;
                var assetPath = AssetDatabase.GetAssetPath(asset);
                source.Validate(context.At(assetPath));
            }

            var referenceValidator = new ProjectDataReferenceValidator();
            referenceValidator.Validate(sources, context);

            var reporter = new UnityConsoleValidationReporter();
            reporter.Report(result);

            if (result.IsValid)
            {
                Debug.Log($"Project data validation succeeded. Checked {assets.Length} assets.");
                return;
            }

            Debug.LogError(
                $"Project data validation failed with {result.Issues.Count} issues " +
                $"across {assets.Length} assets.");
        }
    }
}