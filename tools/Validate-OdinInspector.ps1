param([string]$ProjectAssetsRoot)

$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $PSScriptRoot

if ([string]::IsNullOrWhiteSpace($ProjectAssetsRoot)) {
    $ProjectAssetsRoot = Join-Path $projectRoot "Assets/_Project"
}

$projectAssetsRoot = [IO.Path]::GetFullPath($ProjectAssetsRoot)
$errors = [System.Collections.Generic.List[string]]::new()
$presentationAttributes = @(
    "HideMonoScript",
    "TableList",
    "LabelText",
    "PreviewField",
    "HideLabel",
    "PropertyOrder",
    "TableColumnWidth",
    "DisplayAsString")
$attributePattern = "(?m)^[ \t]*\[(?:[A-Za-z_][A-Za-z0-9_]*\.)?(?<name>" +
    (($presentationAttributes | ForEach-Object { [regex]::Escape($_) }) -join "|") +
    ")(?:Attribute)?(?:\s*\(|\s*\])"

foreach ($file in Get-ChildItem -LiteralPath $projectAssetsRoot -Recurse -Filter "*CatalogConfig.cs") {
    $content = [IO.File]::ReadAllText($file.FullName)
    $isItemIconCatalog = $file.Name -eq "ItemIconCatalogConfig.cs"

    foreach ($attributeMatch in [regex]::Matches($content, $attributePattern)) {
        $attributeName = $attributeMatch.Groups["name"].Value

        if (
            $isItemIconCatalog -and
            $attributeName -in "TableList", "PreviewField", "TableColumnWidth") {
            continue
        }

        $lineNumber = [regex]::Matches(
            $content.Substring(0, $attributeMatch.Index),
            "\r\n|\r|\n").Count + 1
        $errors.Add(
            "Catalogs must use the default Odin presentation: " +
            "$($file.FullName):$lineNumber ($attributeName)")
    }

    if ($isItemIconCatalog) {
        $hasLayeredCatalogField = $content -match
            "(?m)^[ \t]*\[TableList\(AlwaysExpanded\s*=\s*true,\s*DrawScrollView\s*=\s*false\)\]\r?\n" +
                "[ \t]*\[ValidateInput\([^\]\r\n]+\)\]\r?\n[ \t]*\[SerializeField\][ \t]+private\b"
        $hasLayeredSpriteField = $content -match
            "(?m)^[ \t]*\[PreviewField\(48,\s*ObjectFieldAlignment\.Center\),\s*TableColumnWidth\(64\)\]\r?\n" +
                "[ \t]*\[SerializeField,\s*Required\][ \t]+private[ \t]+Sprite\b"

        if ($hasLayeredCatalogField -eq $false -or $hasLayeredSpriteField -eq $false) {
            $errors.Add("Item icon catalog must use the approved compact sprite table: $($file.FullName)")
        }
    }
}

if ($errors.Count -gt 0) {
    foreach ($validationError in $errors) {
        Write-Output "ERROR: $validationError"
    }

    exit 1
}

Write-Output "Odin Inspector validation passed."
