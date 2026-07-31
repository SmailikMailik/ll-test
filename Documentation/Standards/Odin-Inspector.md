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

## Reusable presentation attributes

- Create a composite attribute when one approved presentation recipe is repeated by multiple fields or config
  types and should evolve as a unit. Do not create one merely to shorten a single call site.
- Name the class after the visual concept with the `Attribute` suffix and use it without the suffix, for example
  `SpritePreviewAttribute` and `[SpritePreview]`.
- Place presentation-owned composites under `Assets/_Project/Scripts/Presentation/Inspector` in the
  `LL.Presentation.Inspector` namespace. A capability-specific composite stays in that capability's `Inspector`
  folder instead.
- Build a purely declarative composite with Odin's `[IncludeMyAttributes]`. Do not introduce a custom drawer or
  attribute processor when composing existing Odin attributes is sufficient.
- Keep fixed recipe values inside the composite. Add parameters only when the standard defines multiple meaningful
  variants; do not expose arbitrary per-field styling.
- A composite presentation attribute occupies exactly one presentation-layer line above validation and declaration
  layers.

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
- `SpritePreview`

Functional attributes such as `ValidateInput`, `Required`, `AssetsOnly`, `MinValue`, `ShowInInspector`, and `Button`
remain allowed when they provide real authoring behavior rather than arranging the inspector. Helper members remain
allowed for the same reason.

### Sprite catalog presentation

Apply the compact sprite-catalog presentation to `ItemIconCatalogConfig`, `FlagCatalogConfig`, and
`HeroPortraitCatalogConfig`:

- Render the entries with `[TableList(AlwaysExpanded = true, DrawScrollView = false)]` so each entry occupies one
  row and the complete catalog remains visible without a nested scroll view.
- Keep identifier fields as flexible text columns and label them by their domain identifier type: `Item ID`,
  `Flag ID`, or `Hero ID`. Preserve `ID` capitalization explicitly with `LabelText` rather than relying on field
  name humanization.
- Name identifier fields by the same domain type (`_itemId`, `_flagId`, `_heroId`). Use a generic `_id` only in a
  genuinely generic entry type whose identifier domain is supplied by a type parameter.
- Render every sprite field with `[SpritePreview]`. `SpritePreviewAttribute` composes
  `PreviewField(48, ObjectFieldAlignment.Center)` and `TableColumnWidth(64)`; change those values centrally rather
  than overriding them at a call site.
- Do not add custom labels, groups, colors, titles, or other decoration.

Keep catalog configurations without sprite fields on the default baseline until another presentation rule is
explicitly added here.

## Enforcement

Run the Odin Inspector convention check after changing a catalog inspector or this agreement:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validate-OdinInspector.ps1
```

The script enforces the catalog baseline. Other Odin presentation choices remain review requirements until a
specific rule and reliable check are added here.
