# Project Rules

- C# (`.cs`) files must end immediately after the final non-empty line, with no trailing newline character (`LF` or `CRLF`) at end of file.
- Mark every constructor invoked by the DI container with `[Inject]`, even when it is the type's only constructor or a parameterless constructor. Constructors called explicitly with `new` are manual composition and must not be marked with `[Inject]`.
- DI `Construct` methods must only validate and assign dependencies; do not subscribe, initialize state, update UI, or perform other side effects in them.
- Place each C# attribute on its own line; keep `[SerializeField]` inline with the field declaration.
- Declare serialized fields first in a type, without `[Required]` by default; place constants immediately after them.
- Declare every project enum with `byte` as its underlying type and assign explicit sequential values starting at `0`.
- C# namespaces must mirror the folder structure within their containing assembly.
- Keep code lines within 120 characters; wrap earlier only when it materially improves readability.
- When generating code, prioritize readability and clear separation of responsibilities. Reuse existing code wherever possible, and introduce abstractions when they make repeated use simpler without adding unnecessary complexity.
- Write commit messages in concise English imperative form, starting with a capital letter, without conventional prefixes such as `feat:` or `refactor:`, and without a trailing period.
