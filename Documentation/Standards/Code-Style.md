# C# Code Style and Lifecycle Conventions

This document is the source of truth for project-owned C# style and lifecycle conventions. Architectural ownership,
placement, dependencies, and type-role naming belong to [`Architecture.md`](Architecture.md). Validation semantics
and required checks belong to [`Validation.md`](Validation.md).

These rules apply to C# under `Assets/_Project`. Generated and third-party code is outside their scope.

## Readability and line length

- Treat 120 characters as the point at which wrapping deserves a readability decision.
- Lines from 121 through 140 characters may remain intact when the single-line form is clearer.
- Never exceed 140 characters except for the constructs listed under [Hard-limit exceptions](#hard-limit-exceptions).
- Keep a simple assignment, expression, or invocation with one expression or lambda argument on one line when it
  fits within 120 characters.
- Do not break immediately after an assignment operator merely to shorten a line.
- End every C# file immediately after its final non-empty line, without a trailing `LF` or `CRLF` character.

### Hard-limit exceptions

The following constructs must remain complete on one physical line regardless of length. They are the only
exceptions to the 140-character hard limit:

- A Unity Editor `[MenuItem(...)]` attribute.
- A null-coalescing throw expression containing `?? throw`, from its left operand through the thrown expression.

Add any future hard-limit exception to this list and make narrower sections link here instead of restating the
exception set.

## Expressions and null checks

- Prefer positive predicate names and conditions when they make control flow easier to read. Both unary `!` and an
  explicit `expression is false` pattern are allowed; choose the clearer form at the call site.
- Check ordinary managed references with `is null` and `is not null`, not `== null` or `!= null`.
- Check references whose static type is `UnityEngine.Object` or a derived Unity type with `== null` and `!= null` so
  Unity's destroyed-object fake-null semantics are preserved.
- Keep every null-coalescing throw expression in the form
  `_dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));` on one physical line, as
  required by [Hard-limit exceptions](#hard-limit-exceptions).
- When a dependency is not assigned directly, use a conventional `if` guard followed by
  `throw new ArgumentNullException(...)`.

## Declarations and member organization

- Write every empty C# body inline as `{ }` after its declaration. For a multiline declaration, place `{ }` after
  the final signature line.
- Declare serialized fields first in a type. Do not add `[Required]` by default.
- Place constants immediately after serialized fields. Other fields, properties, constructors, lifecycle methods,
  public or internal behavior, and private helpers follow in that order when practical.
- Declare every project enum with `byte` as its underlying type and explicit sequential values starting at `0`.
  Preserve existing numeric values when an enum is serialized or persisted.
- Use `Min` and `Max`, rather than `Minimum` and `Maximum`, in identifiers that represent bounds. Keep words
  unabbreviated in user-facing text.

## Attributes and serialized fields

- Place attributes applied to types, constructors, methods, properties, and other members on separate lines.
- Follow the single-line `[MenuItem(...)]` rule in [Hard-limit exceptions](#hard-limit-exceptions).
- Keep `[SerializeField]` and `[JsonProperty]` inline with the field declaration.
- Organize a serialized field's attributes into responsibility layers, ordered from inspector presentation to value
  validation to the field declaration. Put each non-empty layer on its own physical line.
- Combine attributes that belong to the same layer in one attribute list. Never combine presentation or value
  validation attributes with the inline declaration layer.
- The declaration layer contains `[SerializeField]` or `[JsonProperty]` and field-contract attributes such as
  `Required` and `AssetsOnly`; keep the complete layer inline with the field declaration.
- Presentation attributes such as `PreviewField`, `TableColumnWidth`, `SuffixLabel`, and `Tooltip` belong on the
  first line. Value constraints such as `ValidateInput`, `Min`, `Max`, `Range`, `MinValue`, and `MaxValue` belong on
  the next line. Omit a layer when it has no attributes.
- For example:

  ```csharp
  [PreviewField(48, ObjectFieldAlignment.Center), TableColumnWidth(64)]
  [SerializeField, Required] private Sprite _icon;
  ```
- Serialized-field migration rules belong to the authored-asset section of `Architecture.md` because they protect
  persisted data rather than formatting.

## Dependency injection

- Mark every constructor selected by VContainer with `[Inject]`, including a sole or parameterless constructor.
- Mark VContainer injection methods named `Construct` with `[Inject]`.
- Constructors called explicitly with `new`, including installer, DTO, snapshot, definition, value-object, and
  factory-product constructors, are manual composition and must not use `[Inject]`.
- A `Construct` method only validates and assigns dependencies. It must not subscribe, initialize state, update UI,
  or perform other side effects.

## Reactive and event lifecycle

- Create long-lived R3 subscriptions owned by a `MonoBehaviour` in `Start` and bind them to the component lifetime
  with `AddTo(this)`.
- Give dynamically created or replaced subscriptions an explicit active lifetime and still dispose them when the
  component is destroyed.
- Name event and reactive-notification callbacks with the `On...` prefix.
- Reserve `Handle...` for non-event command or workflow processing.

## Unity components

- Add `[DisallowMultipleComponent]` to a concrete `MonoBehaviour` when multiple instances on one `GameObject` have no
  defined behavior. Abstract component bases need not declare it.

## Enforcement

Run the code-style check after changing project-owned C#:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validate-CodeStyle.ps1
```

The script enforces only rules that can be checked reliably without interpreting design intent. Rules not covered
by the script remain review requirements; see the coverage table in `Validation.md`.
