# Project Rules

- C# (`.cs`) files must end immediately after the final non-empty line, with no trailing newline character (`LF` or `CRLF`) at end of file.
- Every constructor used by dependency injection must be explicitly marked with `[Inject]`, even when it is the type's only constructor or a parameterless constructor.
- DI `Construct` methods must only validate and assign dependencies; do not subscribe, initialize state, update UI, or perform other side effects in them.
- Place each C# attribute on its own line; keep `[SerializeField]` inline with the field declaration.
- Declare serialized fields first in a type, without `[Required]` by default; place constants immediately after them.
- C# namespaces must mirror the folder structure within their containing assembly.
- Write commit messages in concise English imperative form, starting with a capital letter, without conventional prefixes such as `feat:` or `refactor:`, and without a trailing period.
