# Testing Standard

This document owns project-wide conventions for test placement, fixture design, test data, test doubles, and test
readability. [`Validation.md`](Validation.md) defines which checks a change requires; this document defines how tests
are authored. Commands and environment details remain in [`CI.md`](../Development/CI.md).

## Test assemblies and placement

- Put deterministic runtime, mapping, serialization, and editor-API tests in `Assets/_Project/Tests/EditMode`.
- Use `Assets/_Project/Tests/PlayMode` only when the behavior requires a running player loop, scene loading, or Unity
  component lifecycle that EditMode cannot represent faithfully.
- Below an assembly root, mirror the production owner and capability. For example, tests for `User/Persistence`
  belong in `EditMode/User/Persistence`, while generic save infrastructure belongs in
  `EditMode/Infrastructure/Saving`.
- Match namespaces to physical paths. A fixture in `EditMode/User/Persistence` uses
  `LL.Tests.EditMode.User.Persistence`.
- Keep one fixture per system under test and name its file `<SystemUnderTest>Tests.cs`.
- Create folders for stable production owners or capabilities, or for a coherent group of related test files. Do not
  create a folder merely to repeat one fixture's class name.
- Preserve Unity `.meta` GUIDs when moving existing tests and commit `.meta` files for new folders and files.

The current EditMode organization is:

```text
EditMode/
├── Game/Data
├── Game/Identifiers
├── Infrastructure/Collections
├── Infrastructure/Loading
├── Infrastructure/Saving
├── Infrastructure/Saving/Storage
├── Project
├── TestData
├── User/Defaults
├── User/Persistence
├── User/State/Heroes
└── Validation
```

## Fixture boundaries

- A fixture tests one production type or one indivisible public contract. Split tests when setup, vocabulary, or
  failure modes belong to different owners.
- Name tests after observable behavior, such as `UnsupportedVersionIsResetToDefaultsAndSavedAsCurrentVersion`.
- Keep the scenario linear: arrange the relevant state, perform the behavior, then assert its observable result.
  Separate these phases with blank lines when that improves scanning.
- Keep values that define the scenario visible in the test. Replace unexplained literals with narrowly named
  constants when their meaning or historical role matters.
- Prefer semantic lookups over incidental collection positions unless order is the behavior being tested.
- Assert externally observable results. Do not duplicate the production algorithm inside the test.
- Test both the successful contract and meaningful failure boundaries. For persisted data, cover the current version,
  unsupported versions, mapping, and the chosen reset or rejection policy.

## Test data and doubles

- Put reusable domain setup in `EditMode/TestData` and name it by owner, such as `GameTestData` or `UserTestData`.
  Do not create a generic `Helpers`, `Common`, or catch-all factory.
- A test-data method returns a small valid baseline and accepts only variations used by multiple fixtures. Keep
  scenario-specific construction in the fixture so the important difference remains visible.
- Keep controllable technical dependencies, such as `ManualTimeProvider`, in `TestData` when they are shared across
  owners.
- Place a reusable test double beside the technical contract it implements. For example, `MemorySaveStorage` belongs
  in `Infrastructure/Saving/Storage`.
- Keep a double or context used by one fixture beside that fixture. A context may own repetitive construction and
  cleanup, but it must not hide the action or outcome under test.
- Test helpers must not contain alternative production rules. They assemble inputs, control dependencies, expose
  recorded interactions, and release resources.

## Determinism and cleanup

- Inject controllable time instead of waiting in real time.
- Do not depend on test execution order, existing save files, or state left by another fixture.
- Release subscriptions, Unity objects, temporary files, and other owned resources in `TearDown`, `Dispose`, or a
  `finally` block appropriate to the fixture.
- Use a unique fixture-owned path below the system temporary directory for filesystem tests and clean up only that
  path.
- A test must fail when required setup cannot be established; do not continue after a failed save, load, or lookup.

## What to verify after changing tests

Follow the change matrix in [`Validation.md`](Validation.md). At minimum, test moves and helper changes require
architecture validation, code-style validation, compilation, and the affected EditMode or PlayMode suite. Use the
commands documented in [`CI.md`](../Development/CI.md).
