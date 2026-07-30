param(
    [ValidateSet("EditMode", "PlayMode")]
    [string]$Platform = "EditMode"
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "Common.ps1")

$projectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$artifactsDirectory = Join-Path $projectRoot "artifacts"
New-Item -ItemType Directory -Path $artifactsDirectory -Force | Out-Null

$platformName = $Platform.ToLowerInvariant()

Invoke-UnityEditor -Arguments @(
    "-batchmode",
    "-nographics",
    "-projectPath",
    $projectRoot,
    "-runTests",
    "-testPlatform",
    $Platform,
    "-testResults",
    (Join-Path $artifactsDirectory "$platformName-results.xml"),
    "-logFile",
    (Join-Path $artifactsDirectory "$platformName-tests.log")
)
