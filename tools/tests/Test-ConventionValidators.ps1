$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$architectureValidator = Join-Path $projectRoot "tools/Validate-Architecture.ps1"
$codeStyleValidator = Join-Path $projectRoot "tools/Validate-CodeStyle.ps1"
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
