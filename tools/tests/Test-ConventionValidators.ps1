$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$architectureValidator = Join-Path $projectRoot "tools/Validate-Architecture.ps1"
$codeStyleValidator = Join-Path $projectRoot "tools/Validate-CodeStyle.ps1"
$odinInspectorValidator = Join-Path $projectRoot "tools/Validate-OdinInspector.ps1"
$fixtureRoot = Join-Path ([IO.Path]::GetTempPath()) "ll-convention-validator-$([guid]::NewGuid())"
$failures = [System.Collections.Generic.List[string]]::new()

function Add-TestFailure {
    param([string]$Message)

    $script:failures.Add($Message)
}

function New-ArchitectureFixture {
    param([string]$Name)

    $root = Join-Path $fixtureRoot $Name
    $scriptsRoot = Join-Path $root "Assets/_Project/Scripts"

    foreach ($path in @(
        $scriptsRoot,
        (Join-Path $root "Assets/_Project/Editor"),
        (Join-Path $root "Assets/_Project/Tests"),
        (Join-Path $scriptsRoot "Bootstrap"),
        (Join-Path $scriptsRoot "Composition/Scopes"),
        (Join-Path $scriptsRoot "Composition/Installers"),
        (Join-Path $scriptsRoot "Composition/Factories"),
        (Join-Path $scriptsRoot "Game"),
        (Join-Path $scriptsRoot "Infrastructure"),
        (Join-Path $scriptsRoot "Presentation"),
        (Join-Path $scriptsRoot "UI"),
        (Join-Path $scriptsRoot "User"),
        (Join-Path $scriptsRoot "Validation"))) {
        [IO.Directory]::CreateDirectory($path) | Out-Null
    }

    return $root
}

function Invoke-ExpectedResult {
    param(
        [string]$Name,
        [string]$Script,
        [string[]]$Arguments,
        [int]$ExpectedExitCode,
        [string]$ExpectedOutput
    )

    $output = & powershell -NoProfile -ExecutionPolicy Bypass -File $Script @Arguments 2>&1 | Out-String
    $exitCode = $LASTEXITCODE

    if ($exitCode -ne $ExpectedExitCode) {
        Add-TestFailure "$Name returned $exitCode; expected $ExpectedExitCode. Output: $output"
        return
    }

    if ($output -notmatch [regex]::Escape($ExpectedOutput)) {
        Add-TestFailure "$Name did not report '$ExpectedOutput'. Output: $output"
    }
}

try {
    [IO.Directory]::CreateDirectory($fixtureRoot) | Out-Null

    $validArchitectureRoot = New-ArchitectureFixture "architecture-valid"
    [IO.Directory]::CreateDirectory(
        (Join-Path $validArchitectureRoot "Assets/_Project/Scripts/Infrastructure/Loading")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $validArchitectureRoot "Assets/_Project/Scripts/Infrastructure/Loading/CompiledDataLoader.cs"),
        "namespace LL.Infrastructure.Loading;`n`ninternal sealed class CompiledDataLoader : IDataLoader<int> { }")
    [IO.Directory]::CreateDirectory(
        (Join-Path $validArchitectureRoot "Assets/_Project/Scripts/User/Persistence")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $validArchitectureRoot "Assets/_Project/Scripts/User/Persistence/UserSessionLoader.cs"),
        "namespace LL.User.Persistence;`n`ninternal sealed class UserSessionLoader : IDataLoader<int> { }")
    Invoke-ExpectedResult `
        -Name "Valid architecture" `
        -Script $architectureValidator `
        -Arguments @("-ProjectRoot", $validArchitectureRoot) `
        -ExpectedExitCode 0 `
        -ExpectedOutput "Architecture validation passed."

    $invalidArchitectureRoot = New-ArchitectureFixture "architecture-invalid-folder"
    [IO.Directory]::CreateDirectory(
        (Join-Path $invalidArchitectureRoot "Assets/_Project/Scripts/Common")) | Out-Null
    Invoke-ExpectedResult `
        -Name "Unsupported architecture folder" `
        -Script $architectureValidator `
        -Arguments @("-ProjectRoot", $invalidArchitectureRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Unsupported runtime top-level area"

    $invalidDomainLoaderRoot = New-ArchitectureFixture "architecture-invalid-domain-loader"
    [IO.File]::WriteAllText(
        (Join-Path $invalidDomainLoaderRoot "Assets/_Project/Scripts/Game/DirectLoader.cs"),
        "namespace LL.Game;`n`ninternal sealed class DirectLoader : IDataLoader<int> { }")
    Invoke-ExpectedResult `
        -Name "Game domain loader outside aggregate boundary" `
        -Script $architectureValidator `
        -Arguments @("-ProjectRoot", $invalidDomainLoaderRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Game domain data may use IDataLoader only at GameDataSnapshot boundary"

    $invalidUserCommandsRoot = New-ArchitectureFixture "architecture-invalid-ui-user-commands"
    [IO.File]::WriteAllText(
        (Join-Path $invalidUserCommandsRoot "Assets/_Project/Scripts/UI/DirectUserMutation.cs"),
        "using LL.User.State.Heroes;`nusing LL.User.State.RankUp;`n`nnamespace LL.UI;`n`n" +
            "internal sealed class DirectUserMutation`n{`n    private IUserHeroProgressCommands _progress;`n" +
            "    private IUserRankUpAttemptsCommands _attempts;`n}")
    Invoke-ExpectedResult `
        -Name "UI user hero commands" `
        -Script $architectureValidator `
        -Arguments @("-ProjectRoot", $invalidUserCommandsRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "UI must mutate user state through game services"

    $missingContentMenuRoot = New-ArchitectureFixture "architecture-missing-content-menu"
    [IO.Directory]::CreateDirectory(
        (Join-Path $missingContentMenuRoot "Assets/_Project/Scripts/Game/Configuration")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $missingContentMenuRoot "Assets/_Project/Scripts/Game/Configuration/ExampleConfig.cs"),
        "using UnityEngine;`n`nnamespace LL.Game.Configuration`n{`n    [CreateAssetMenu]`n" +
            "    internal sealed class ExampleConfig : ScriptableObject { }`n}")
    Invoke-ExpectedResult `
        -Name "Missing Content menu entry" `
        -Script $architectureValidator `
        -Arguments @("-ProjectRoot", $missingContentMenuRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Root authored config must be exposed through Last Level/Content"

    $validStyleRoot = Join-Path $fixtureRoot "style-valid"
    [IO.Directory]::CreateDirectory((Join-Path $validStyleRoot "Scripts/Game")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $validStyleRoot "Scripts/Game/Mode.cs"),
        "namespace LL.Game;`n`ninternal enum Mode : byte`n{`n    First = 0,`n    Second = 1`n}")
    [IO.Directory]::CreateDirectory((Join-Path $validStyleRoot "Editor/Menu")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $validStyleRoot "Editor/Menu/ProjectMenu.cs"),
        "using UnityEditor;`n`ninternal static class ProjectMenu`n{`n" +
            "    [MenuItem(`"Project/Deliberately Long Menu Item That Must Remain On One Physical Line Regardless Of The General Hard Line Limit`", false, 123)]`n" +
            "    private static void Run() { }`n}")
    [IO.File]::WriteAllText(
        (Join-Path $validStyleRoot "Scripts/Game/LongGuard.cs"),
        "using System;`n`ninternal sealed class LongGuard`n{`n" +
            "    private readonly object _dependencyWithAnIntentionallyLongNameForTestingTheHardLimitException;`n`n" +
            "    internal LongGuard(object dependencyWithAnIntentionallyLongNameForTestingTheHardLimitException)`n    {`n" +
            "        _dependencyWithAnIntentionallyLongNameForTestingTheHardLimitException = dependencyWithAnIntentionallyLongNameForTestingTheHardLimitException ?? throw new ArgumentNullException(nameof(dependencyWithAnIntentionallyLongNameForTestingTheHardLimitException));`n    }`n}")
    Invoke-ExpectedResult `
        -Name "Valid code style" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $validStyleRoot) `
        -ExpectedExitCode 0 `
        -ExpectedOutput "Code-style validation passed."

    $invalidStyleRoot = Join-Path $fixtureRoot "style-invalid-enum"
    [IO.Directory]::CreateDirectory((Join-Path $invalidStyleRoot "Scripts/Game")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidStyleRoot "Scripts/Game/Mode.cs"),
        "namespace LL.Game;`n`ninternal enum Mode`n{`n    First`n}")
    Invoke-ExpectedResult `
        -Name "Invalid enum style" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidStyleRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Enum must use byte"

    $invalidUnityNullRoot = Join-Path $fixtureRoot "style-invalid-unity-null"
    [IO.Directory]::CreateDirectory((Join-Path $invalidUnityNullRoot "Scripts/UI")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidUnityNullRoot "Scripts/UI/View.cs"),
        "using UnityEngine;`n`ninternal sealed class View`n{`n    private Transform _target;`n" +
            "    private bool IsMissing => _target is null;`n}")
    Invoke-ExpectedResult `
        -Name "Invalid Unity null pattern" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidUnityNullRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Unity object must use overloaded equality"

    $invalidCallbackRoot = Join-Path $fixtureRoot "style-invalid-callback"
    [IO.Directory]::CreateDirectory((Join-Path $invalidCallbackRoot "Scripts/UI")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidCallbackRoot "Scripts/UI/View.cs"),
        "internal sealed class View`n{`n    private void Observe(dynamic source)`n    {`n" +
            "        source.Subscribe(UpdateValue);`n    }`n`n    private void UpdateValue(int value) { }`n}")
    Invoke-ExpectedResult `
        -Name "Invalid reactive callback" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidCallbackRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Reactive callback method must use the On prefix"

    $invalidBoundNameRoot = Join-Path $fixtureRoot "style-invalid-bound-name"
    [IO.Directory]::CreateDirectory((Join-Path $invalidBoundNameRoot "Scripts/Game")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidBoundNameRoot "Scripts/Game/Limit.cs"),
        "internal static class Limit`n{`n    private const int ExperienceMaximum = 1;`n}")
    Invoke-ExpectedResult `
        -Name "Invalid bound identifier" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidBoundNameRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Bound identifier must use Min or Max"

    $invalidFileEndingRoot = Join-Path $fixtureRoot "style-invalid-file-ending"
    [IO.Directory]::CreateDirectory((Join-Path $invalidFileEndingRoot "Scripts/Game")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidFileEndingRoot "Scripts/Game/Value.cs"),
        "internal readonly struct Value { }`n")
    Invoke-ExpectedResult `
        -Name "Invalid C# file ending" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidFileEndingRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "C# file must end without a trailing newline"

    $invalidMenuItemRoot = Join-Path $fixtureRoot "style-invalid-menu-item"
    [IO.Directory]::CreateDirectory((Join-Path $invalidMenuItemRoot "Editor/Menu")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidMenuItemRoot "Editor/Menu/ProjectMenu.cs"),
        "using UnityEditor;`n`ninternal static class ProjectMenu`n{`n    [MenuItem(`n" +
            "        `"Project/Action`",`n        false,`n        0)]`n    private static void Run() { }`n}")
    Invoke-ExpectedResult `
        -Name "Invalid MenuItem layout" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidMenuItemRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "MenuItem attribute must remain on one physical line"

    $invalidThrowExpressionRoot = Join-Path $fixtureRoot "style-invalid-throw-expression"
    [IO.Directory]::CreateDirectory((Join-Path $invalidThrowExpressionRoot "Scripts/Game")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidThrowExpressionRoot "Scripts/Game/Guard.cs"),
        "using System;`n`ninternal sealed class Guard`n{`n    private readonly object _dependency;`n`n" +
            "    internal Guard(object dependency)`n    {`n        _dependency = dependency ??`n" +
            "            throw new ArgumentNullException(nameof(dependency));`n    }`n}")
    Invoke-ExpectedResult `
        -Name "Invalid null-coalescing throw layout" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidThrowExpressionRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Null-coalescing throw expression must remain on one physical line"

    $invalidMultilineThrowRoot = Join-Path $fixtureRoot "style-invalid-multiline-throw"
    [IO.Directory]::CreateDirectory((Join-Path $invalidMultilineThrowRoot "Scripts/Game")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidMultilineThrowRoot "Scripts/Game/Guard.cs"),
        "using System;`n`ninternal sealed class Guard`n{`n    private readonly object _dependency;`n`n" +
            "    internal Guard(object dependency)`n    {`n        _dependency = dependency ?? throw new ArgumentNullException(`n" +
            "            nameof(dependency));`n    }`n}")
    Invoke-ExpectedResult `
        -Name "Invalid multiline thrown expression" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidMultilineThrowRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Null-coalescing throw expression must remain on one physical line"

    $invalidAttributeLayerRoot = Join-Path $fixtureRoot "style-invalid-attribute-layer"
    [IO.Directory]::CreateDirectory((Join-Path $invalidAttributeLayerRoot "Scripts/Game")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidAttributeLayerRoot "Scripts/Game/Config.cs"),
        "using UnityEngine;`n`ninternal sealed class Config`n{`n" +
            "    [SerializeField, Min(0)] private int _value;`n}")
    Invoke-ExpectedResult `
        -Name "Invalid serialized field attribute layer" `
        -Script $codeStyleValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidAttributeLayerRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Presentation and value-validation attributes must use separate layers"

    $validOdinRoot = Join-Path $fixtureRoot "odin-valid-catalog"
    [IO.Directory]::CreateDirectory((Join-Path $validOdinRoot "Scripts/Game/Configuration")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $validOdinRoot "Scripts/Game/Configuration/ExampleCatalogConfig.cs"),
        "using Sirenix.OdinInspector;`n`ninternal sealed class ExampleCatalogConfig`n{`n" +
            "    [ValidateInput(nameof(IsValid))]`n    private object[] _entries;`n`n" +
            "    private static bool IsValid(object[] entries) => true;`n}")
    Invoke-ExpectedResult `
        -Name "Valid default catalog presentation" `
        -Script $odinInspectorValidator `
        -Arguments @("-ProjectAssetsRoot", $validOdinRoot) `
        -ExpectedExitCode 0 `
        -ExpectedOutput "Odin Inspector validation passed."

    $validItemIconOdinRoot = Join-Path $fixtureRoot "odin-valid-item-icon-catalog"
    [IO.Directory]::CreateDirectory((Join-Path $validItemIconOdinRoot "Scripts/Presentation/Items/Configuration")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $validItemIconOdinRoot "Scripts/Presentation/Items/Configuration/ItemIconCatalogConfig.cs"),
        "using Sirenix.OdinInspector;`n`ninternal sealed class ItemIconCatalogConfig`n{`n" +
            "    [TableList(AlwaysExpanded = true, DrawScrollView = false)]`n" +
            "    [ValidateInput(nameof(IsValid))]`n    [SerializeField] private ItemIconEntry[] _icons;`n}`n`n" +
            "internal sealed class ItemIconEntry`n{`n    [LabelText(`"Item ID`")]`n    [SerializeField] private string _itemId;`n" +
            "    [SpritePreview]`n" +
            "    [SerializeField, Required] private Sprite _icon;`n}")
    Invoke-ExpectedResult `
        -Name "Valid compact item icon catalog" `
        -Script $odinInspectorValidator `
        -Arguments @("-ProjectAssetsRoot", $validItemIconOdinRoot) `
        -ExpectedExitCode 0 `
        -ExpectedOutput "Odin Inspector validation passed."

    $validFlagOdinRoot = Join-Path $fixtureRoot "odin-valid-flag-catalog"
    [IO.Directory]::CreateDirectory((Join-Path $validFlagOdinRoot "Scripts/Presentation/Flags/Configuration")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $validFlagOdinRoot "Scripts/Presentation/Flags/Configuration/FlagCatalogConfig.cs"),
        "using Sirenix.OdinInspector;`n`ninternal sealed class FlagCatalogConfig`n{`n" +
            "    [TableList(AlwaysExpanded = true, DrawScrollView = false)]`n" +
            "    [ValidateInput(nameof(IsValid))]`n    [SerializeField] private FlagEntry[] _flags;`n}`n`n" +
            "internal sealed class FlagEntry`n{`n    [LabelText(`"Flag ID`")]`n    [SerializeField] private string _flagId;`n" +
            "    [SpritePreview]`n" +
            "    [SerializeField, Required] private Sprite _flag;`n}")
    Invoke-ExpectedResult `
        -Name "Valid compact flag catalog" `
        -Script $odinInspectorValidator `
        -Arguments @("-ProjectAssetsRoot", $validFlagOdinRoot) `
        -ExpectedExitCode 0 `
        -ExpectedOutput "Odin Inspector validation passed."

    $invalidOdinRoot = Join-Path $fixtureRoot "odin-invalid-catalog"
    [IO.Directory]::CreateDirectory((Join-Path $invalidOdinRoot "Scripts/Game/Configuration")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidOdinRoot "Scripts/Game/Configuration/ExampleCatalogConfig.cs"),
        "using Sirenix.OdinInspector;`n`ninternal sealed class ExampleCatalogConfig`n{`n" +
            "    [TableList]`n    private object[] _entries;`n}")
    Invoke-ExpectedResult `
        -Name "Invalid catalog presentation" `
        -Script $odinInspectorValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidOdinRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Catalogs must use the default Odin presentation"

    $invalidItemIconOdinRoot = Join-Path $fixtureRoot "odin-invalid-item-icon-catalog"
    [IO.Directory]::CreateDirectory((Join-Path $invalidItemIconOdinRoot "Scripts/Presentation/Items/Configuration")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidItemIconOdinRoot "Scripts/Presentation/Items/Configuration/ItemIconCatalogConfig.cs"),
        "internal sealed class ItemIconCatalogConfig`n{`n    private object[] _icons;`n}")
    Invoke-ExpectedResult `
        -Name "Missing compact item icon presentation" `
        -Script $odinInspectorValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidItemIconOdinRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Sprite catalog must use the approved compact sprite table"

    $invalidFlagOdinRoot = Join-Path $fixtureRoot "odin-invalid-flag-catalog"
    [IO.Directory]::CreateDirectory((Join-Path $invalidFlagOdinRoot "Scripts/Presentation/Flags/Configuration")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $invalidFlagOdinRoot "Scripts/Presentation/Flags/Configuration/FlagCatalogConfig.cs"),
        "internal sealed class FlagCatalogConfig`n{`n    private object[] _flags;`n}")
    Invoke-ExpectedResult `
        -Name "Missing compact flag presentation" `
        -Script $odinInspectorValidator `
        -Arguments @("-ProjectAssetsRoot", $invalidFlagOdinRoot) `
        -ExpectedExitCode 1 `
        -ExpectedOutput "Sprite catalog must use the approved compact sprite table"
} finally {
    $resolvedFixtureRoot = [IO.Path]::GetFullPath($fixtureRoot)
    $resolvedTempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())

    if ($resolvedFixtureRoot.StartsWith($resolvedTempRoot) -and [IO.Directory]::Exists($resolvedFixtureRoot)) {
        Remove-Item -LiteralPath $resolvedFixtureRoot -Recurse -Force
    }
}

if ($failures.Count -gt 0) {
    foreach ($failure in $failures) {
        Write-Output "ERROR: $failure"
    }

    exit 1
}

Write-Output "Convention validator tests passed."
exit 0
