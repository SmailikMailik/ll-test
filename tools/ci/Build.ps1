param(
    [string]$Target = "StandaloneWindows64",
    [string]$Output,
    [string]$Version,
    [int]$BuildNumber = 0
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "Common.ps1")

$projectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$artifactsDirectory = Join-Path $projectRoot "artifacts"
New-Item -ItemType Directory -Path $artifactsDirectory -Force | Out-Null

$env:LL_BUILD_OUTPUT = $Output
$env:LL_BUILD_VERSION = $Version
$env:LL_BUILD_NUMBER = if ($BuildNumber -gt 0) { $BuildNumber.ToString() } else { $null }

try {
    Invoke-UnityEditor -Arguments @(
        "-batchmode",
        "-nographics",
        "-quit",
        "-projectPath",
        $projectRoot,
        "-buildTarget",
        $Target,
        "-executeMethod",
        "LLEditor.CI.CiEntryPoint.Build",
        "-logFile",
        (Join-Path $artifactsDirectory "unity-build.log")
    )
}
finally {
    Remove-Item Env:LL_BUILD_OUTPUT -ErrorAction SilentlyContinue
    Remove-Item Env:LL_BUILD_VERSION -ErrorAction SilentlyContinue
    Remove-Item Env:LL_BUILD_NUMBER -ErrorAction SilentlyContinue
}
