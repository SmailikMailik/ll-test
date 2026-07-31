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

function Test-RuntimeDependencies {
    foreach ($file in Get-ChildItem -LiteralPath $scriptsRoot -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)
        $namespaceMatch = [regex]::Match(
            $content,
            "(?m)^namespace\s+(LL\.(?<area>[A-Za-z_][A-Za-z0-9_]*)(?:\.[A-Za-z0-9_.]+)?)")

        if ($namespaceMatch.Success -eq $false) {
            continue
        }

        $sourceNamespace = $namespaceMatch.Groups[1].Value
        $sourceArea = $namespaceMatch.Groups["area"].Value

        foreach ($usingMatch in [regex]::Matches(
            $content,
            "(?m)^using\s+LL\.(?<area>[A-Za-z_][A-Za-z0-9_]*)(?<suffix>[A-Za-z0-9_.]*);")) {
            $targetArea = $usingMatch.Groups["area"].Value
            $targetNamespace = "LL.$targetArea$($usingMatch.Groups["suffix"].Value)"

            if ($targetArea -eq $sourceArea) {
                continue
            }

            $allowed = switch ($sourceArea) {
                "Composition" { $true; break }
                "Bootstrap" {
                    $targetNamespace -eq "LL.UI.Controls"
                    break
                }
                "UI" {
                    if ($targetArea -in @("Game", "Presentation", "User")) {
                        $true
                    } elseif (
                        $targetArea -in @("Infrastructure", "Validation") -and
                        $sourceNamespace -like "LL.UI.Windows.Configuration*") {
                        $true
                    } else {
                        $false
                    }

                    break
                }
                "Presentation" {
                    if ($targetArea -in @("Game", "User")) {
                        $true
                    } elseif (
                        $targetArea -in @("Infrastructure", "Validation") -and
                        $sourceNamespace -like "LL.Presentation.*.Configuration*") {
                        $true
                    } else {
                        $false
                    }

                    break
                }
                "User" {
                    $targetArea -in @("Game", "Infrastructure", "Validation")
                    break
                }
                "Game" {
                    if ($targetArea -eq "Validation") {
                        $true
                    } elseif (
                        $targetArea -eq "User" -and
                        $sourceNamespace -like "LL.Game.*.Services*") {
                        $true
                    } elseif (
                        $targetArea -eq "Infrastructure" -and
                        (
                            $sourceNamespace -like "LL.Game.Data*" -or
                            $sourceNamespace -like "LL.Game.*.Configuration*"
                        )) {
                        $true
                    } else {
                        $false
                    }

                    break
                }
                "Infrastructure" {
                    $targetArea -eq "Validation"
                    break
                }
                "Validation" { $false; break }
                default { $false }
            }

            if ($allowed -eq $false) {
                Add-ArchitectureError (
                    "Forbidden runtime dependency: $sourceNamespace -> $targetNamespace " +
                    "in $($file.FullName)")
            }
        }
    }
}

function Test-EnumDeclarations {
    foreach ($file in Get-ChildItem -LiteralPath $scriptsRoot -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)
        $enumPattern = "\benum\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)" +
            "(?:\s*:\s*(?<base>[A-Za-z_][A-Za-z0-9_]*))?\s*\{(?<body>[^{}]*)\}"

        foreach ($enumMatch in [regex]::Matches(
            $content,
            $enumPattern,
            [Text.RegularExpressions.RegexOptions]::Singleline)) {
            $enumName = $enumMatch.Groups["name"].Value

            if ($enumMatch.Groups["base"].Value -ne "byte") {
                Add-ArchitectureError "Enum must use byte: $($file.FullName) ($enumName)"
                continue
            }

            $expectedValue = 0

            foreach ($rawMember in $enumMatch.Groups["body"].Value.Split(",")) {
                $member = $rawMember.Trim()

                if ($member.Length -eq 0) {
                    continue
                }

                $memberMatch = [regex]::Match(
                    $member,
                    "^(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<value>[0-9]+)$")

                if (
                    $memberMatch.Success -eq $false -or
                    [int]$memberMatch.Groups["value"].Value -ne $expectedValue) {
                    Add-ArchitectureError (
                        "Enum values must be explicit and sequential from 0: " +
                        "$($file.FullName) ($enumName)")
                    break
                }

                $expectedValue++
            }
        }
    }
}

function Test-DiRegistrations {
    $typeFiles = @{}

    foreach ($file in Get-ChildItem -LiteralPath $scriptsRoot -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)

        foreach ($typeMatch in [regex]::Matches(
            $content,
            "\b(?:class|struct)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)")) {
            $typeFiles[$typeMatch.Groups["name"].Value] = $file
        }
    }

    foreach ($file in Get-ChildItem -LiteralPath $compositionRoot -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)
        $registrationPattern = "\bbuilder\s*\.\s*Register(?:EntryPoint)?<" +
            "(?<name>[A-Za-z_][A-Za-z0-9_]*)>"

        foreach ($registration in [regex]::Matches($content, $registrationPattern)) {
            $typeName = $registration.Groups["name"].Value

            if ($typeFiles.ContainsKey($typeName) -eq $false) {
                continue
            }

            $typeFile = $typeFiles[$typeName]
            $typeContent = [IO.File]::ReadAllText($typeFile.FullName)

            if ($typeContent -notmatch "\[Inject\]") {
                Add-ArchitectureError (
                    "DI-created type must mark its constructor or Construct method with [Inject]: " +
                    "$($typeFile.FullName) ($typeName)")
            }
        }
    }
}

function Test-DomainDataLoaders {
    foreach ($file in Get-ChildItem -LiteralPath (Join-Path $scriptsRoot "Game") -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)

        if (
            $content -match "IDataLoader<" -and
            $file.Name -ne "GameDataLoader.cs") {
            Add-ArchitectureError (
                "Game domain data may use IDataLoader only at GameDataSnapshot boundary: " +
                "$($file.FullName)")
        }
    }

    foreach ($file in Get-ChildItem -LiteralPath (Join-Path $scriptsRoot "User") -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)

        if (
            $content -match "IDataLoader<" -and
            $file.Name -notin @("UserDefaultsLoader.cs", "UserSessionLoader.cs")) {
            Add-ArchitectureError (
                "User domain data may use IDataLoader only at UserDefaultsSnapshot or UserSnapshot boundary: " +
                "$($file.FullName)")
        }
    }
}

function Test-UserStateCommandConsumers {
    $commandsPattern = "\bIUser(?:Items|Progress|RankUpQuest)Commands\b"

    foreach ($file in Get-ChildItem -LiteralPath (Join-Path $scriptsRoot "UI") -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)

        if ($content -match $commandsPattern) {
            Add-ArchitectureError (
                "UI must mutate user state through game services, not User.State command interfaces: " +
                "$($file.FullName)")
        }
    }
}

$allowedCompositionFolders = @("Scopes", "Installers", "Factories")
$allowedRuntimeFolders = @(
    "Bootstrap",
    "Composition",
    "Game",
    "Infrastructure",
    "Presentation",
    "UI",
    "User",
    "Validation")

foreach ($directory in Get-ChildItem -LiteralPath $scriptsRoot -Directory) {
    if ($directory.Name -notin $allowedRuntimeFolders) {
        Add-ArchitectureError "Unsupported runtime top-level area: $($directory.FullName)"
    }
}

foreach ($file in Get-ChildItem -LiteralPath $scriptsRoot -File -Filter "*.cs") {
    if ($file.Name -ne "AssemblyInfo.cs") {
        Add-ArchitectureError "C# file must belong to a runtime area: $($file.FullName)"
    }
}

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
Test-RuntimeDependencies
Test-EnumDeclarations
Test-DiRegistrations
Test-DomainDataLoaders
Test-UserStateCommandConsumers

foreach ($file in Get-ChildItem (Join-Path $projectRoot "Assets/_Project") -Recurse -Filter "*.cs") {
    $content = [IO.File]::ReadAllText($file.FullName)
    $bytes = [IO.File]::ReadAllBytes($file.FullName)

    if ($bytes.Length -gt 0 -and $bytes[$bytes.Length - 1] -in 10, 13) {
        Add-ArchitectureError "C# file has a trailing newline: $($file.FullName)"
    }

    if ($content -match "\bFormerlySerializedAs\s*\(") {
        Add-ArchitectureError "FormerlySerializedAs is not allowed; migrate serialized keys: $($file.FullName)"
    }

    if ($content -match "(?m)^[ \t]*\[[^\]\r\n]+\][ \t]*\[[^\]\r\n]+\]") {
        Add-ArchitectureError "Place each C# attribute on its own line: $($file.FullName)"
    }

    if ($content -match "\)\s*\r?\n\s*\{\s*\r?\n\s*\}") {
        Add-ArchitectureError "Empty C# bodies must be inline: $($file.FullName)"
    }

    foreach ($assignment in [regex]::Matches(
        $content,
        "(?m)^(?<indent>[ \t]*)(?<left>[^\r\n=]+=[ \t]*)\r?\n[ \t]+(?<right>[^\r\n;]+;)")) {
        $singleLine = (
            $assignment.Groups["indent"].Value +
            $assignment.Groups["left"].Value.TrimEnd() +
            " " +
            $assignment.Groups["right"].Value.Trim())

        if ($singleLine.Length -le 120) {
            Add-ArchitectureError (
                "Simple assignment that fits within 120 characters must remain on one line: " +
                "$($file.FullName)")
        }
    }

    foreach ($invocation in [regex]::Matches(
        $content,
        "(?m)^(?<indent>[ \t]*)(?<head>[^\r\n]+\()\r?\n[ \t]+(?<tail>[^\r\n]+?\);)[ \t]*$")) {
        $singleLine = (
            $invocation.Groups["indent"].Value +
            $invocation.Groups["head"].Value.TrimEnd() +
            $invocation.Groups["tail"].Value.Trim())

        if ($singleLine.Length -le 120) {
            Add-ArchitectureError (
                "Simple invocation that fits within 120 characters must remain on one line: " +
                "$($file.FullName)")
        }
    }

    if (
        $content -match "\binternal\s+sealed\s+class\s+\w+\s*:\s*" +
            "(?:MonoBehaviour|MaskableGraphic|Selectable)\b" -and
        $content -notmatch "\[DisallowMultipleComponent\]") {
        Add-ArchitectureError (
            "Concrete component should declare [DisallowMultipleComponent]: $($file.FullName)")
    }

    if ($file.FullName.StartsWith($scriptsRoot) -and $content -match "\.Subscribe\s*\(") {
        foreach ($subscription in [regex]::Matches($content, "\.Subscribe\s*\(")) {
            $remainingLength = [Math]::Min(1000, $content.Length - $subscription.Index)
            $subscriptionText = $content.Substring($subscription.Index, $remainingLength)

            if (
                $file.FullName -like "*\Scripts\UI\*" -and
                $subscriptionText -notmatch "\.AddTo\s*\(\s*this\s*\)") {
                Add-ArchitectureError (
                    "UI R3 subscription must be bound to the component lifetime: " +
                    "$($file.FullName)")
                break
            }
        }
    }

    $lineNumber = 0

    foreach ($line in [IO.File]::ReadLines($file.FullName)) {
        $lineNumber++

        if ($line.Length -gt 140) {
            Add-ArchitectureError "Line exceeds the hard limit of 140 characters: $($file.FullName):$lineNumber"
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
