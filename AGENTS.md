# Project Rules

- Treat approved production code and serialized assets as the source of truth for current behavior and actual
  placement. Treat `ARCHITECTURE.md` as the source of truth for intended architectural conventions, and keep it
  synchronized with approved code and asset layout. Apply its placement procedure before creating or moving a type.
- Validators enforce approved code, asset, and architecture decisions; they do not define those decisions. Do not
  move production code or assets solely to satisfy a stale validator. Resolve the intended design first, then update
  the validator and `ARCHITECTURE.md` to match it.
- After changing paths, namespaces, serialized shapes, dependencies, data flow, identifiers, or domain invariants,
  review and run every affected validator. Update validation discovery, rules, and tests in the same change so the
  validation system continues to check the current implementation.
- Keep composition code under exactly one of `Assets/_Project/Scripts/Composition/Scopes`, `Installers`, or
  `Factories`. Scopes define lifetime boundaries, installers implement cohesive `IInstaller` registration modules,
  and factories construct concrete policies without performing registration.
- Do not introduce vague architectural folders or names such as `Common`, `Misc`, `Helpers`, `Managers`, `Runtime`,
  or an unqualified `Data`. If no documented category fits, resolve ownership first and update `ARCHITECTURE.md`
  when establishing a genuinely new project-wide convention.
- Keep `ARCHITECTURE.md` consistent when a change introduces a new top-level area, architectural role, dependency
  direction, or naming convention.
- Run `tools/Validate-Architecture.ps1` after adding or moving project code; resolve every reported violation.
- C# (`.cs`) files must end immediately after the final non-empty line, with no trailing newline character (`LF` or `CRLF`) at end of file.
- Mark every constructor invoked by the DI container with `[Inject]`, even when it is the type's only constructor or a parameterless constructor. Constructors called explicitly with `new` are manual composition and must not be marked with `[Inject]`.
- DI `Construct` methods must only validate and assign dependencies; do not subscribe, initialize state, update UI, or perform other side effects in them.
- Create long-lived R3 subscriptions in a `MonoBehaviour`'s `Start` method and bind them to the component lifetime with `AddTo(this)`. For subscriptions created dynamically, control their active lifetime explicitly and still ensure they are disposed when the component is destroyed.
- Name event and reactive-notification callback methods with the `On...` prefix;
  reserve `Handle...` for non-event command or workflow processing.
- Place each C# attribute on its own line, except serialized-field constraints and decorators such as `Min`, `Max`,
  `Range`, and `Tooltip`: keep them in the same attribute list after `SerializeField`, for example
  `[SerializeField, Min(0f)] private float _duration;`. Keep `[SerializeField]` and `[JsonProperty]` inline with
  the field declaration.
- When renaming a serialized field, explicitly migrate its key in every affected scene, prefab, and asset, verify
  that the old key no longer exists, and do not use `FormerlySerializedAs`.
- Use `Min` and `Max` instead of `Minimum` and `Maximum` in identifiers that represent lower and upper bounds.
  Keep unabbreviated words in user-facing text.
- Write every empty C# body inline as `{ }` after its declaration. This applies to types, methods, constructors,
  local functions, operators, and accessors. For a multiline declaration, place `{ }` after its final signature
  line. Do not expand an empty body across separate lines.
- Do not use the unary `!` operator to negate Boolean expressions in project C# code; write an explicit
  `expression is false` pattern instead. This rule does not apply to the `!=` inequality operator or preprocessor
  expressions, where C# pattern syntax is unavailable.
- Check ordinary managed references for null with `is null` and `is not null`, not `== null` or `!= null`. Check
  `UnityEngine.Object` instances and derived Unity objects with `== null` and `!= null` so destroyed-object fake-null
  semantics are preserved; do not use `is null` or `is not null` for them.
- Add `[DisallowMultipleComponent]` to `MonoBehaviour` components when multiple instances on one `GameObject` have no valid use.
- Declare serialized fields first in a type, without `[Required]` by default; place constants immediately after them.
- Declare every project enum with `byte` as its underlying type and assign explicit sequential values starting at `0`.
- C# namespaces must mirror the folder structure within their containing assembly.
- Keep configuration types with the subsystem whose data they define or construct; do not centralize unrelated
  configurations under `Presentation`. Use `Presentation/Configuration` only for presentation-specific data.
- Treat 120 characters as a readability review threshold, not a mandatory wrap point. For lines from 121 through
  140 characters, keep the line intact when that is clearer and wrap it when the split improves readability. Never
  exceed the hard limit of 140 characters. Keep simple assignments and expressions on one line when they fit within
  120 characters; in particular, do not break immediately after an assignment operator unless the multiline form
  materially clarifies the expression. Apply the same rule to a simple invocation with a single expression or lambda
  argument: do not leave the opening parenthesis at the end of one line and the entire argument on the next when the
  invocation fits within 120 characters.
- In DI constructors and `[Inject]` `Construct` methods, keep a dependency null guard that directly assigns the
  dependency in the form `_dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));` on a
  single line, even when it exceeds the 140-character hard limit. When the dependency is not assigned directly, use
  a conventional two-line `if` statement followed by `throw new ArgumentNullException(...)`.
- When generating code, prioritize readability and clear separation of responsibilities. Reuse existing code wherever possible, and introduce abstractions when they make repeated use simpler without adding unnecessary complexity.
- When asked for a commit message, output only a concise English imperative phrase as plain text: start with a capital letter and do not use quotation marks, backticks, explanations, conventional prefixes such as `feat:` or `refactor:`, or a trailing period.
