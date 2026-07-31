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
    "DisplayAsString",
    "SpritePreview")
$spriteCatalogNames = @(
    "ItemIconCatalogConfig.cs",
    "FlagCatalogConfig.cs",
    "HeroPortraitCatalogConfig.cs")
$spriteCatalogIdentifierLabels = @{
    "ItemIconCatalogConfig.cs" = @{ Label = "Item ID"; Field = "_itemId" }
    "FlagCatalogConfig.cs" = @{ Label = "Flag ID"; Field = "_flagId" }
    "HeroPortraitCatalogConfig.cs" = @{ Label = "Hero ID"; Field = "_heroId" }
}
$attributePattern = "(?m)^[ \t]*\[(?:[A-Za-z_][A-Za-z0-9_]*\.)?(?<name>" +
    (($presentationAttributes | ForEach-Object { [regex]::Escape($_) }) -join "|") +
    ")(?:Attribute)?(?:\s*\(|\s*\])"

foreach ($file in Get-ChildItem -LiteralPath $projectAssetsRoot -Recurse -Filter "*CatalogConfig.cs") {
    $content = [IO.File]::ReadAllText($file.FullName)
    $isSpriteCatalog = $file.Name -in $spriteCatalogNames

    foreach ($attributeMatch in [regex]::Matches($content, $attributePattern)) {
        $attributeName = $attributeMatch.Groups["name"].Value

        if (
            $isSpriteCatalog -and
            $attributeName -in "TableList", "LabelText", "SpritePreview") {
            continue
        }

        $lineNumber = [regex]::Matches(
            $content.Substring(0, $attributeMatch.Index),
            "\r\n|\r|\n").Count + 1
        $errors.Add(
            "Catalogs must use the default Odin presentation: " +
            "$($file.FullName):$lineNumber ($attributeName)")
    }

    if ($isSpriteCatalog) {
        $identifier = $spriteCatalogIdentifierLabels[$file.Name]
        $hasLayeredCatalogField = $content -match
            "(?m)^[ \t]*\[TableList\(AlwaysExpanded\s*=\s*true,\s*DrawScrollView\s*=\s*false\)\]\r?\n" +
                "[ \t]*\[ValidateInput\([^\]\r\n]+\)\]\r?\n[ \t]*\[SerializeField\][ \t]+private\b"
        $spriteFieldCount = [regex]::Matches(
            $content,
            "(?m)^[ \t]*\[SerializeField,\s*Required\][ \t]+private[ \t]+Sprite\b").Count
        $layeredSpriteFieldCount = [regex]::Matches(
            $content,
            "(?m)^[ \t]*\[SpritePreview\]\r?\n" +
                "[ \t]*\[SerializeField,\s*Required\][ \t]+private[ \t]+Sprite\b").Count
        $identifierPattern =
            '(?m)^[ \t]*\[LabelText\("' + [regex]::Escape($identifier.Label) + '"\)\]\r?\n' +
            '[ \t]*\[SerializeField\][ \t]+private[ \t]+string[ \t]+' +
            [regex]::Escape($identifier.Field) + ';'
        $hasTypedIdentifierLabel = $content -match $identifierPattern

        if (
            $hasLayeredCatalogField -eq $false -or
            $hasTypedIdentifierLabel -eq $false -or
            $spriteFieldCount -eq 0 -or
            $layeredSpriteFieldCount -ne $spriteFieldCount) {
            $errors.Add("Sprite catalog must use the approved compact sprite table: $($file.FullName)")
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
