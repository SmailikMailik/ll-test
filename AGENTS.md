# Project Rules

- C# (`.cs`) files must end immediately after the final non-empty line, with no trailing newline character (`LF` or `CRLF`) at end of file.
- Mark every constructor invoked by the DI container with `[Inject]`, even when it is the type's only constructor or a parameterless constructor. Constructors called explicitly with `new` are manual composition and must not be marked with `[Inject]`.
- DI `Construct` methods must only validate and assign dependencies; do not subscribe, initialize state, update UI, or perform other side effects in them.
- Create long-lived R3 subscriptions in a `MonoBehaviour`'s `Start` method and bind them to the component lifetime with `AddTo(this)`. For subscriptions created dynamically, control their active lifetime explicitly and still ensure they are disposed when the component is destroyed.
- Name event and reactive-notification callback methods with the `On...` prefix;
  reserve `Handle...` for non-event command or workflow processing.
- Place each C# attribute on its own line; keep `[SerializeField]` and `[JsonProperty]` inline with the field declaration.
- Add `[DisallowMultipleComponent]` to `MonoBehaviour` components when multiple instances on one `GameObject` have no valid use.
- Declare serialized fields first in a type, without `[Required]` by default; place constants immediately after them.
- Declare every project enum with `byte` as its underlying type and assign explicit sequential values starting at `0`.
- C# namespaces must mirror the folder structure within their containing assembly.
- Keep configuration types with the subsystem whose data they define or construct; do not centralize unrelated
  configurations under `Presentation`. Use `Presentation/Configuration` only for presentation-specific data.
- Keep code lines within 120 characters; wrap earlier only when it materially improves readability.
- When generating code, prioritize readability and clear separation of responsibilities. Reuse existing code wherever possible, and introduce abstractions when they make repeated use simpler without adding unnecessary complexity.
- When asked for a commit message, output only a concise English imperative phrase as plain text: start with a capital letter and do not use quotation marks, backticks, explanations, conventional prefixes such as `feat:` or `refactor:`, or a trailing period.
