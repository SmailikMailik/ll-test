$ErrorActionPreference = "Stop"

function Get-UnityEditorPath {
    if ([string]::IsNullOrWhiteSpace($env:UNITY_EDITOR_PATH) -eq $false) {
        if (Test-Path -LiteralPath $env:UNITY_EDITOR_PATH) {
            return (Resolve-Path $env:UNITY_EDITOR_PATH).Path
        }

        throw "UNITY_EDITOR_PATH does not exist: $env:UNITY_EDITOR_PATH"
    }

    $projectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $versionFile = Join-Path $projectRoot "ProjectSettings/ProjectVersion.txt"
    $versionLine = Get-Content -LiteralPath $versionFile |
        Where-Object { $_ -like "m_EditorVersion:*" } |
        Select-Object -First 1
    $version = $versionLine.Split(":")[1].Trim()
    $candidates = @(
        "D:/Programs/Unity/$version/Editor/Unity.exe",
        "C:/Program Files/Unity/Hub/Editor/$version/Editor/Unity.exe"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path -LiteralPath $candidate) {
            return (Resolve-Path $candidate).Path
        }
    }

    throw "Unity $version was not found. Set UNITY_EDITOR_PATH."
}

function Invoke-UnityEditor {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    $unityEditor = Get-UnityEditorPath
    $escapedArguments = $Arguments | ForEach-Object {
        if ($_ -match '\s') {
            '"' + $_.Replace('"', '\"') + '"'
        }
        else {
            $_
        }
    }
    $process = Start-Process `
        -FilePath $unityEditor `
        -ArgumentList $escapedArguments `
        -WindowStyle Hidden `
        -Wait `
        -PassThru

    if ($process.ExitCode -ne 0) {
        throw "Unity exited with code $($process.ExitCode)."
    }
}
