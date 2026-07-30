$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot
$scriptsRoot = Join-Path $projectRoot "Assets/_Project/Scripts"
$editorRoot = Join-Path $projectRoot "Assets/_Project/Editor"
$testsRoot = Join-Path $projectRoot "Assets/_Project/Tests"
$compositionRoot = Join-Path $scriptsRoot "Composition"
$errors = [System.Collections.Generic.List[string]]::new()

function Add-ArchitectureError {
    param([string]$Message)

    $script:errors.Add($Message)
}

function Test-NamespaceTree {
    param(
        [string]$Root,
        [string]$RootNamespace
    )

    foreach ($file in Get-ChildItem -LiteralPath $Root -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)
        $namespaceMatch = [regex]::Match($content, "(?m)^namespace\s+([A-Za-z_][A-Za-z0-9_.]*)")

        if ($namespaceMatch.Success -eq $false) {
            if ($file.Name -ne "AssemblyInfo.cs") {
                Add-ArchitectureError "Missing namespace: $($file.FullName)"
            }

            continue
        }

        $relativeDirectory = $file.DirectoryName.Substring($Root.Length).TrimStart([char[]]@("\", "/"))
        $expectedNamespace = $RootNamespace

        if ($relativeDirectory.Length -gt 0) {
            $namespaceSuffix = $relativeDirectory.Replace(
                [IO.Path]::DirectorySeparatorChar,
                [char]".")
            $expectedNamespace = "$RootNamespace.$namespaceSuffix"
        }

        $actualNamespace = $namespaceMatch.Groups[1].Value

        if ($actualNamespace -ne $expectedNamespace) {
            Add-ArchitectureError (
                "Namespace mismatch: $($file.FullName) uses '$actualNamespace'; " +
                "expected '$expectedNamespace'.")
        }
    }
}

$allowedCompositionFolders = @("Scopes", "Installers", "Factories")

foreach ($directory in Get-ChildItem -LiteralPath $compositionRoot -Directory) {
    if ($directory.Name -notin $allowedCompositionFolders) {
        Add-ArchitectureError "Unsupported Composition folder: $($directory.FullName)"
    }
}

foreach ($file in Get-ChildItem -LiteralPath $compositionRoot -File -Filter "*.cs") {
    Add-ArchitectureError "C# files must not be placed directly in Composition: $($file.FullName)"
}

foreach ($file in Get-ChildItem -LiteralPath $compositionRoot -Recurse -Filter "*LifetimeScope.cs") {
    if ($file.Directory.Name -ne "Scopes") {
        Add-ArchitectureError "Lifetime scope must be under Composition/Scopes: $($file.FullName)"
    }
}

foreach ($file in Get-ChildItem -LiteralPath $compositionRoot -Recurse -Filter "*Installer.cs") {
    if ($file.Directory.Name -ne "Installers") {
        Add-ArchitectureError "Installer must be under Composition/Installers: $($file.FullName)"
    }

    $content = [IO.File]::ReadAllText($file.FullName)

    if ($content -notmatch ":\s*IInstaller\b") {
        Add-ArchitectureError "Installer must implement IInstaller: $($file.FullName)"
    }
}

foreach ($file in Get-ChildItem -LiteralPath $compositionRoot -Recurse -Filter "*Factory.cs") {
    if ($file.Directory.Name -ne "Factories") {
        Add-ArchitectureError "Factory must be under Composition/Factories: $($file.FullName)"
    }
}

$vagueFolderNames = @("Common", "Misc", "Helpers", "Managers", "Runtime")

foreach ($sourceRoot in @($scriptsRoot, $editorRoot)) {
    foreach ($directory in Get-ChildItem -LiteralPath $sourceRoot -Recurse -Directory) {
        if ($directory.Name -in $vagueFolderNames) {
            Add-ArchitectureError "Vague architectural folder name: $($directory.FullName)"
        }
    }
}

Test-NamespaceTree -Root $scriptsRoot -RootNamespace "LL"
Test-NamespaceTree -Root $editorRoot -RootNamespace "LLEditor"
Test-NamespaceTree -Root $testsRoot -RootNamespace "LL.Tests"

foreach ($file in Get-ChildItem (Join-Path $projectRoot "Assets/_Project") -Recurse -Filter "*.cs") {
    $bytes = [IO.File]::ReadAllBytes($file.FullName)

    if ($bytes.Length -gt 0 -and $bytes[$bytes.Length - 1] -in 10, 13) {
        Add-ArchitectureError "C# file has a trailing newline: $($file.FullName)"
    }

    $lineNumber = 0

    foreach ($line in [IO.File]::ReadLines($file.FullName)) {
        $lineNumber++

        if ($line.Length -gt 120) {
            Add-ArchitectureError "Line exceeds 120 characters: $($file.FullName):$lineNumber"
        }
    }
}

if ($errors.Count -gt 0) {
    foreach ($architectureError in $errors) {
        Write-Output "ERROR: $architectureError"
    }

    exit 1
}

Write-Output "Architecture validation passed."
