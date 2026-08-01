# Project Rules

Before changing the project, use the agreement that owns the decision:

- [`Architecture.md`](Documentation/Standards/Architecture.md) defines ownership, placement, dependency direction,
  data boundaries, naming,
  composition, and authored-asset layout.
- [`Code-Style.md`](Documentation/Standards/Code-Style.md) defines C# formatting, member organization, DI and reactive
  lifecycle conventions,
  and Unity component conventions.
- [`Odin-Inspector.md`](Documentation/Standards/Odin-Inspector.md) defines Odin-powered inspector presentation and
  authoring UI conventions.
- [`Validation.md`](Documentation/Standards/Validation.md) defines validation semantics, validator ownership,
  automated-rule coverage, and the checks required for each kind of change.
- [`Testing.md`](Documentation/Standards/Testing.md) defines test placement, fixture boundaries, test data and doubles,
  readability, determinism, and cleanup conventions.
- [`CI.md`](Documentation/Development/CI.md) defines the commands and environment used to run validation, tests, and
  builds.

## Agreement ownership

Each project-wide convention has exactly one owning agreement file. Other documents link to that agreement instead
of repeating its normative rules. `AGENTS.md` is the concise index and working contract, not a second copy of the
agreements.

Create a separate agreement only when a subject has a distinct responsibility, vocabulary, and change cadence that
would make an existing agreement harder to navigate. Keep a short or feature-local rule in the nearest existing
agreement or feature document. When splitting or moving rules, preserve their meaning, update every inbound link,
and remove the old normative copy in the same change.

Approved production code and serialized assets are the source of truth for current behavior and actual placement.
The agreements above are the source of truth for intended conventions. Validators enforce approved decisions; they
do not establish them. Resolve the intended design before changing production code to satisfy a stale validator,
then update the applicable agreement, validator, and tests together.

Apply the placement procedure in `Documentation/Standards/Architecture.md` before creating or moving a type. Preserve
Unity `.meta` GUIDs and follow that agreement's serialized-identity and field-migration rules when moving or renaming
Unity types, assets, or fields.

After a change, run every check required by the change matrix in `Documentation/Standards/Validation.md`. An
architectural change must update the architecture agreement and every affected overview, validator, and validator
test in the same change.

When generating code, prioritize readability and clear separation of responsibilities. Reuse existing code where
possible, and introduce an abstraction only when it creates a meaningful boundary, replacement point, or reuse
point.

When asked for a commit message, output only a concise English imperative phrase as plain text: start with a capital
letter and do not use quotation marks, backticks, explanations, conventional prefixes, or a trailing period.
