param([string]$ProjectAssetsRoot)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot

if ([string]::IsNullOrWhiteSpace($ProjectAssetsRoot)) {
    $ProjectAssetsRoot = Join-Path $projectRoot "Assets/_Project"
}

$projectAssetsRoot = [IO.Path]::GetFullPath($ProjectAssetsRoot)
$scriptsRoot = Join-Path $projectAssetsRoot "Scripts"
$errors = [System.Collections.Generic.List[string]]::new()

function Add-CodeStyleError {
    param([string]$Message)

    $script:errors.Add($Message)
}

function Remove-CSharpCommentsAndLiterals {
    param([string]$Content)

    $pattern = @'
(?msx)
    ^[ \t]*\#[^\r\n]*
  | //[^\r\n]*
  | /\*.*?\*/
  | (?:\$@|@\$|@)"(?:""|[^"])*"
  | \$?"(?:\\.|[^"\\])*"
  | '(?:\\.|[^'\\])*'
'@

    return [regex]::Replace(
        $Content,
        $pattern,
        { param($match) [regex]::Replace($match.Value, "[^\r\n]", " ") })
}

function Test-UnityNullPatterns {
    $unityTypeNames = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)

    foreach ($typeName in @(
        "Object",
        "GameObject",
        "Transform",
        "RectTransform",
        "Component",
        "MonoBehaviour",
        "ScriptableObject",
        "Sprite",
        "Texture",
        "Texture2D",
        "Texture3D",
        "Material",
        "Camera",
        "Canvas",
        "CanvasGroup",
        "Animator",
        "Animation",
        "AudioClip",
        "AudioSource",
        "Graphic",
        "MaskableGraphic",
        "Selectable",
        "Image",
        "Button",
        "TMP_Text",
        "TextMeshProUGUI",
        "SceneAsset")) {
        $unityTypeNames.Add($typeName) | Out-Null
    }

    $files = @(Get-ChildItem -LiteralPath $projectAssetsRoot -Recurse -Filter "*.cs")
    $addedType = $true

    while ($addedType) {
        $addedType = $false

        foreach ($file in $files) {
            $codeOnly = Remove-CSharpCommentsAndLiterals ([IO.File]::ReadAllText($file.FullName))

            foreach ($typeMatch in [regex]::Matches(
                $codeOnly,
                "\bclass\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)[^\r\n{]*:\s*" +
                    "(?:[A-Za-z_][A-Za-z0-9_]*\.)*(?<base>[A-Za-z_][A-Za-z0-9_]*)")) {
                if (
                    $unityTypeNames.Contains($typeMatch.Groups["base"].Value) -and
                    $unityTypeNames.Add($typeMatch.Groups["name"].Value)) {
                    $addedType = $true
                }
            }
        }
    }

    $typePattern = ($unityTypeNames | ForEach-Object { [regex]::Escape($_) }) -join "|"

    foreach ($file in $files) {
        $content = [IO.File]::ReadAllText($file.FullName)
        $codeOnly = Remove-CSharpCommentsAndLiterals $content
        $declarationPattern = "\b(?:$typePattern)(?:\[\])?\s+(?<name>_[A-Za-z0-9_]+|[a-z][A-Za-z0-9_]*)\b"

        foreach ($declaration in [regex]::Matches($codeOnly, $declarationPattern)) {
            $name = [regex]::Escape($declaration.Groups["name"].Value)

            foreach ($nullMatch in [regex]::Matches(
                $codeOnly,
                "\b$name\s+is\s+(?:not\s+)?null\b")) {
                $lineNumber = [regex]::Matches(
                    $codeOnly.Substring(0, $nullMatch.Index),
                    "\r\n|\r|\n").Count + 1
                Add-CodeStyleError (
                    "Unity object must use overloaded equality for null checks: " +
                    "$($file.FullName):$lineNumber")
            }
        }
    }
}

function Test-EnumDeclarations {
    foreach ($file in Get-ChildItem -LiteralPath $projectAssetsRoot -Recurse -Filter "*.cs") {
        $content = [IO.File]::ReadAllText($file.FullName)
        $enumPattern = "\benum\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)" +
            "(?:\s*:\s*(?<base>[A-Za-z_][A-Za-z0-9_]*))?\s*\{(?<body>[^{}]*)\}"

        foreach ($enumMatch in [regex]::Matches(
            $content,
            $enumPattern,
            [Text.RegularExpressions.RegexOptions]::Singleline)) {
            $enumName = $enumMatch.Groups["name"].Value

            if ($enumMatch.Groups["base"].Value -ne "byte") {
                Add-CodeStyleError "Enum must use byte: $($file.FullName) ($enumName)"
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
                    Add-CodeStyleError (
                        "Enum values must be explicit and sequential from 0: " +
                        "$($file.FullName) ($enumName)")
                    break
                }

                $expectedValue++
            }
        }
    }
}

Test-EnumDeclarations
Test-UnityNullPatterns

foreach ($file in Get-ChildItem -LiteralPath $projectAssetsRoot -Recurse -Filter "*.cs") {
    $content = [IO.File]::ReadAllText($file.FullName)
    $bytes = [IO.File]::ReadAllBytes($file.FullName)
    $codeOnly = Remove-CSharpCommentsAndLiterals $content

    if ($bytes.Length -gt 0 -and $bytes[$bytes.Length - 1] -in 10, 13) {
        Add-CodeStyleError "C# file must end without a trailing newline: $($file.FullName)"
    }

    foreach ($menuItemMatch in [regex]::Matches(
        $content,
        "(?s)\[MenuItem\s*\([^\]]*?\)\]")) {
        if ($menuItemMatch.Value -match "\r|\n") {
            $lineNumber = [regex]::Matches(
                $content.Substring(0, $menuItemMatch.Index),
                "\r\n|\r|\n").Count + 1
            Add-CodeStyleError (
                "MenuItem attribute must remain on one physical line: " +
                "$($file.FullName):$lineNumber")
        }
    }

    foreach ($callbackMatch in [regex]::Matches(
        $codeOnly,
        "\.Subscribe\s*\(\s*(?<name>(?!On)[A-Za-z_][A-Za-z0-9_]*)\s*\)")) {
        $lineNumber = [regex]::Matches(
            $codeOnly.Substring(0, $callbackMatch.Index),
            "\r\n|\r|\n").Count + 1
        Add-CodeStyleError (
            "Reactive callback method must use the On prefix: " +
            "$($file.FullName):$lineNumber ($($callbackMatch.Groups["name"].Value))")
    }

    foreach ($boundNameMatch in [regex]::Matches(
        $codeOnly,
        "\b[A-Za-z_][A-Za-z0-9_]*(?:Minimum|Maximum)[A-Za-z0-9_]*\b")) {
        $lineNumber = [regex]::Matches(
            $codeOnly.Substring(0, $boundNameMatch.Index),
            "\r\n|\r|\n").Count + 1
        Add-CodeStyleError (
            "Bound identifier must use Min or Max: " +
            "$($file.FullName):$lineNumber ($($boundNameMatch.Value))")
    }

    if ($content -match "(?m)^[ \t]*\[[^\]\r\n]+\][ \t]*\[[^\]\r\n]+\]") {
        Add-CodeStyleError "Place each C# attribute on its own line: $($file.FullName)"
    }

    if ($content -match "\)\s*\r?\n\s*\{\s*\r?\n\s*\}") {
        Add-CodeStyleError "Empty C# bodies must be inline: $($file.FullName)"
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
            Add-CodeStyleError (
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
            Add-CodeStyleError (
                "Simple invocation that fits within 120 characters must remain on one line: " +
                "$($file.FullName)")
        }
    }

    if (
        $content -match "\binternal\s+sealed\s+class\s+\w+\s*:\s*" +
            "(?:MonoBehaviour|MaskableGraphic|Selectable)\b" -and
        $content -notmatch "\[DisallowMultipleComponent\]") {
        Add-CodeStyleError (
            "Concrete component should declare [DisallowMultipleComponent]: $($file.FullName)")
    }

    if ($file.FullName.StartsWith($scriptsRoot) -and $content -match "\.Subscribe\s*\(") {
        foreach ($subscription in [regex]::Matches($content, "\.Subscribe\s*\(")) {
            $remainingLength = [Math]::Min(1000, $content.Length - $subscription.Index)
            $subscriptionText = $content.Substring($subscription.Index, $remainingLength)

            if (
                $file.FullName -like "*\Scripts\UI\*" -and
                $subscriptionText -notmatch "\.AddTo\s*\(\s*this\s*\)") {
                Add-CodeStyleError (
                    "UI R3 subscription must be bound to the component lifetime: " +
                    "$($file.FullName)")
                break
            }
        }
    }

    $lineNumber = 0

    foreach ($line in [IO.File]::ReadLines($file.FullName)) {
        $lineNumber++

        if ($line.Length -gt 140 -and $line -notmatch "^\s*\[MenuItem\s*\(") {
            Add-CodeStyleError "Line exceeds the hard limit of 140 characters: $($file.FullName):$lineNumber"
        }
    }
}

if ($errors.Count -gt 0) {
    foreach ($codeStyleError in $errors) {
        Write-Output "ERROR: $codeStyleError"
    }

    exit 1
}

Write-Output "Code-style validation passed."
