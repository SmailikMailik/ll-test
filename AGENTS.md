# Project Rules

- C# (`.cs`) files must end immediately after the final non-empty line, with no trailing newline character (`LF` or `CRLF`) at end of file.
- Every constructor used by dependency injection must be explicitly marked with `[Inject]`, even when it is the type's only constructor or a parameterless constructor.
