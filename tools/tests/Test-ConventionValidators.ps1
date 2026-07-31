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

    $validStyleRoot = Join-Path $fixtureRoot "style-valid"
    [IO.Directory]::CreateDirectory((Join-Path $validStyleRoot "Scripts/Game")) | Out-Null
    [IO.File]::WriteAllText(
        (Join-Path $validStyleRoot "Scripts/Game/Mode.cs"),
        "namespace LL.Game;`n`ninternal enum Mode : byte`n{`n    First = 0,`n    Second = 1`n}")
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
