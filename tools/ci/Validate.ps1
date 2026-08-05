$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "Common.ps1")

$projectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$artifactsDirectory = Join-Path $projectRoot "artifacts"
New-Item -ItemType Directory -Path $artifactsDirectory -Force | Out-Null

& (Join-Path $projectRoot "tools/tests/Test-ConventionValidators.ps1")

if ($LASTEXITCODE -ne 0) {
    throw "Convention validator tests failed."
}

& (Join-Path $projectRoot "tools/Validate-Architecture.ps1")

if ($LASTEXITCODE -ne 0) {
    throw "Architecture validation failed."
}

& (Join-Path $projectRoot "tools/Validate-CodeStyle.ps1")

if ($LASTEXITCODE -ne 0) {
    throw "Code-style validation failed."
}

& (Join-Path $projectRoot "tools/Validate-OdinInspector.ps1")

if ($LASTEXITCODE -ne 0) {
    throw "Odin Inspector validation failed."
}

Invoke-UnityEditor -Arguments @(
    "-batchmode",
    "-nographics",
    "-quit",
    "-projectPath",
    $projectRoot,
    "-executeMethod",
    "LLEditor.CI.CiEntryPoint.Validate",
    "-logFile",
    (Join-Path $artifactsDirectory "unity-validation.log")
)
