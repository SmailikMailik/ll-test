# Odin Inspector Authoring Standard

This document is the source of truth for project-owned Odin Inspector presentation and authoring UI conventions.
C# formatting belongs to [`Code-Style.md`](Code-Style.md), authored-asset ownership and placement to
[`Architecture.md`](Architecture.md), and validation coverage to [`Validation.md`](Validation.md).

These rules apply to project C# under `Assets/_Project`. Third-party code and Odin's own configuration are outside
their scope.

## Attribute responsibilities

- Treat an Odin attribute as presentation-only when it changes grouping, ordering, labels, sizing, previews, or
  other visual layout without adding authoring behavior or a data constraint.
- Treat buttons, inspector callbacks, validation, selection constraints, value constraints, and deliberately
  exposed helper members as functional authoring behavior. Do not remove or restyle them as part of a visual-layout
  cleanup unless a narrower rule explicitly covers them.
- Keep the serialized model independent of inspector presentation. Adding or removing a presentation-only attribute
  must not change serialized field names, types, or data.
- Add visual conventions incrementally in this document. Do not copy an isolated layout from another inspector
  before the corresponding shared rule has been approved.
- Apply the serialized-field attribute layers defined in [`Code-Style.md`](Code-Style.md): Odin presentation
  attributes occupy the presentation layer, Odin value checks occupy the validation layer, and field-contract
  attributes remain in the inline declaration layer.

## Catalog baseline

Catalog configuration assets and their serialized entry types use the default inspector layout. Do not apply these
presentation-only Odin attributes in a `*CatalogConfig.cs` file:

- `HideMonoScript`
- `TableList`
- `LabelText`
- `PreviewField`
- `HideLabel`
- `PropertyOrder`
- `TableColumnWidth`
- `DisplayAsString`

Functional attributes such as `ValidateInput`, `Required`, `AssetsOnly`, `MinValue`, `ShowInInspector`, and `Button`
remain allowed when they provide real authoring behavior rather than arranging the inspector. Helper members remain
allowed for the same reason.

### Item icon catalog pilot

`ItemIconCatalogConfig` is the reference implementation for a compact sprite catalog while this presentation is
being evaluated:

- Render the entries with `[TableList(AlwaysExpanded = true, DrawScrollView = false)]` so each entry occupies one
  row and the complete catalog remains visible without a nested scroll view.
- Keep the ID as the default flexible text column.
- Render the sprite with `PreviewField(48, ObjectFieldAlignment.Center)` in a column whose initial width is 64 pixels.
- Keep `PreviewField` and `TableColumnWidth` together in the presentation layer above the field declaration.
- Do not add custom labels, groups, colors, titles, or other decoration.

This pilot applies only to `ItemIconCatalogConfig`. Keep other catalog configurations on the default baseline until
the presentation has been reviewed and its scope is explicitly expanded here.

## Enforcement

Run the Odin Inspector convention check after changing a catalog inspector or this agreement:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validate-OdinInspector.ps1
```

The script enforces the catalog baseline. Other Odin presentation choices remain review requirements until a
specific rule and reliable check are added here.
