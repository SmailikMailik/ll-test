# Project Architecture and Naming Standard

This document is the source of truth for placing and naming project code. Its purpose is to make ownership,
dependency direction, and the role of a type understandable from its path and name.

The rules describe the intended architecture, not merely the current directory tree. New code must follow them.
When existing code is changed substantially, move it toward this standard when that can be done safely within the
task scope.

## Core principles

1. Organize by ownership first and technical role second.
2. Dependencies point from outer, technical layers toward stable domain concepts.
3. A folder and a type suffix must communicate one specific responsibility.
4. Add a layer or abstraction only when it creates a real boundary, replacement point, or reuse point.
5. Do not mirror an identical folder tree across features whose responsibilities are different.
6. Keep Unity authoring models, serialized transport models, domain models, and live state distinct.

## Placement procedure

Before creating or moving a type, answer these questions in order:

1. Who owns the concept: `Game`, `User`, `Presentation`, `UI`, `Infrastructure`, `Validation`, `Composition`, or
   `Editor`?
2. Within that owner, which feature or capability owns it?
3. Is the type a core concept of that feature, or does it have a recognized technical role listed below?
4. Does its proposed dependency direction comply with this document?
5. Does its name use the most specific standard suffix?

Place a core feature type at the feature root. Create a technical-role subfolder only for a coherent boundary or a
group of closely related types. Do not create a one-type folder merely to make the tree look symmetrical.

## Top-level ownership

### `Composition`

Owns application assembly: concrete implementation selection, manual object construction, DI registrations, and
lifetime configuration.

- `Scopes` contains VContainer lifetime scopes. A scope stores serialized Unity references, defines a lifetime
  boundary, and orchestrates installers plus only small local registrations.
- `Installers` contains cohesive registration modules that implement VContainer's `IInstaller`. Construct installers
  manually from a scope; do not resolve them through DI.
- `Factories` contains concrete construction policies that are reused by installers, scopes, or editor tooling.
  Factories construct objects but never register them.
- Do not place C# files directly in `Composition` and do not add other composition subfolders without updating this
  standard.
- Composition may depend on every runtime area.
- Runtime areas must never depend on `Composition`.
- Composition contains no domain decisions, mapping, validation rules, I/O implementation, or mutable application
  state.
- An installer owns one cohesive registration module, such as game-data loading or the user lifecycle. Do not create
  an installer for a single trivial registration.
- A factory method exposes its concrete policy in its name, such as `CreateFromJsonFile` or `CreateJsonFile`.

### `Bootstrap`

Owns the application's first-scene startup flow and temporary loading presentation.

- The Bootstrap scene is the first enabled build scene and transitions to the first application scene.
- The bootstrap flow may display initialization progress and coordinate scene activation.
- Independent startup work is modeled as parallel bootstrap operations collected and coordinated by one bootstrap
  flow.
- It must not create a second project lifetime scope when VContainer already auto-creates the configured root scope.
- It must not own game rules, user persistence, or long-lived application state.

### `Game`

Owns game rules and game concepts: identifiers, definitions, catalogs, calculations, progression, rewards, payments,
and use-case services.

- Organize first by game capability, for example `Ranks`, `Quests`, or `Rewards`.
- Core game models must not depend on `User`, `Presentation`, `UI`, `Composition`, or `Editor`.
- Game application services may coordinate user state through focused `User.State` interfaces. Domain definitions and
  calculations must remain independent of concrete user-state implementations.
- Unity-dependent authoring adapters are allowed under the owning capability's `Configuration` folder.
- Cross-capability workflows belong to the capability that owns the outcome; create a new capability only when no
  existing owner is correct.
- `Game/Data` is reserved for the aggregate game-data loading boundary. It is not a general dumping ground.

### `User`

Owns user-specific defaults, persisted user representation, immutable loaded snapshots, and live mutable user state.

- `Configuration` owns authored defaults and their validation.
- `Persistence` owns user save/load orchestration, save DTO mapping, and version handling.
- `Snapshots` owns immutable point-in-time user data.
- `State` owns long-lived mutable runtime state, grouped by capability.
- `User` may depend on stable `Game` value objects and definitions.

### `Infrastructure`

Owns reusable technical mechanisms such as loading contracts, serialization, storage, and platform adapters.

- Infrastructure must not encode game, user, presentation, or UI policy.
- Infrastructure must not depend on `Game`, `User`, `Presentation`, `UI`, `Composition`, or `Editor`.
- Prefer capability folders such as `Saving/Serialization` and `Saving/Storage` over technology-only folders.
- A technology name belongs on the concrete implementation, for example `JsonSaveSerializer` or
  `PlayerPrefsSaveStorage`.

### `Presentation`

Owns transformations and adapters that turn domain meaning into display meaning: localization, formatting, display
catalogs, and presentation-facing confirmations.

- Presentation may depend on `Game`, `User`, and generic UI primitives needed to implement an adapter.
- Presentation must not own reusable visual controls, concrete feature views, navigation, or game rules.
- Presentation-specific configuration stays with its presentation capability.

### `UI`

Owns concrete visual behavior: views, windows, controls, graphics, navigation, visual state, and visual flows.

- UI may depend on `Presentation`, `Game`, and `User`.
- `MonoBehaviour`, visual components, and window implementations normally belong here.
- Reusable controls belong in a role folder such as `Controls`, `Graphics`, or `VisualStates`.
- Feature-specific views stay under their feature or window view hierarchy.
- UI must not contain persistence, storage, or domain rules.

### `Validation`

Owns reusable validation primitives, results, contexts, rules, and reporting contracts.

- Generic validation belongs in runtime `Scripts/Validation`.
- Validation of one configuration type stays beside that configuration.
- Cross-asset and project-wide validation that uses `AssetDatabase` belongs in `Editor/Validation`.
- Validation reports errors; it must not silently repair source data.

### `Editor`

Owns Unity Editor-only tooling, menus, inspectors, build checks, and asset-database integration.

- Runtime code must never depend on `Editor`.
- Mirror a runtime capability below `Editor` when the tool is feature-specific.
- Keep project-wide editor workflows in a role folder such as `Menu`, `Creation`, or `Validation`.
- `Toolbar` contains compact controls for frequently used project workflows in Unity's main or window toolbars.
  Unity-version-specific toolbar integration stays isolated here and delegates behavior to editor workflow commands.
- `CI` contains provider-independent batch-mode entry points for validation, tests, and player builds. Provider
  configuration calls these entry points and must not duplicate their project rules.

## Recognized role folders

Use these names only with the stated meaning:

- `Configuration`: Unity-authored or otherwise author-authored input for one owning capability.
- `Persistence`: durable/external serialized representation, version handling, mapping, and save/load orchestration.
- `Snapshots`: immutable point-in-time representation composed at a loading boundary.
- `State`: live mutable runtime state.
- `Sources`: source-specific adapters that obtain and construct a model directly.
- `Serialization`: object-to-byte or object-to-text format conversion.
- `Storage`: raw byte or text access to a medium, addressed without domain knowledge.
- `Services`: cohesive domain or application operations with no more specific feature role.
- `Views`: concrete visual representations.
- `Flows`: multi-step application or UI workflows.
- `Extensions`: extension methods only.
- `Reporting`: formatting or delivery of diagnostic and validation results.
- `Scopes`: DI lifetime boundaries and their serialized Unity references; only under `Composition`.
- `Installers`: cohesive `IInstaller` registration modules; only under `Composition`.
- `Factories`: reusable concrete construction policies; under `Composition` when they select application technology.

Avoid vague folders such as `Common`, `Misc`, `Helpers`, `Managers`, `Runtime`, or `Data` without a narrowly documented
meaning. A broadly reusable type still needs a concrete owner and responsibility.

## Type naming vocabulary

Use the most specific applicable suffix:

- `LifetimeScope`: VContainer lifetime boundary hosted by a Unity component.
- `Installer`: cohesive VContainer registration module implementing `IInstaller`.
- `Factory`: constructs a concrete object graph without registering it or retaining its runtime ownership.
- `Config`: root authoring object, normally a `ScriptableObject`.
- `Entry`: one serialized authoring row nested under a configuration.
- `DocumentEntry`: one serialized transport row nested under a `Document`.
- `Definition`: immutable domain description used by game rules.
- `Catalog`: immutable indexed collection of definitions or resources.
- `Snapshot`: immutable, internally consistent point-in-time aggregate.
- `Document`: versioned top-level serialized representation of reference or master data.
- `SaveData`: serialized persisted user-state DTO or one of its owned parts.
- `Mapper`: pure conversion between representations; it performs no I/O and owns no state.
- `Loader`: performs one load operation and returns a fully constructed result.
- `Storage`: reads and writes raw bytes or text to one storage technology.
- `Serializer`: converts between an object representation and bytes or text.
- `Provider`: provides access to an already available resource or a stable creation policy without exposing storage
  details.
- `Source`: represents a concrete origin of data when neither `Loader`, `Storage`, nor `Provider` describes it more
  precisely.
- `State`: mutable runtime representation.
- `Service`: cohesive domain or application operation that does not fit a more specific role.
- `Controller`: coordinates lifecycle, commands, and side effects for an owned process.
- `Flow`: coordinates a multi-step user or application workflow.
- `BootstrapOperation`: one independently started and polled unit of temporary bootstrap work that reports progress,
  readiness, and failure without coordinating other operations.
- `View`: concrete visual representation.
- `Validator`: checks input and reports issues without changing it.
- `Formatter`: converts a value into presentation text or tokens without I/O.

Interfaces use the same role name with an `I` prefix. Concrete technology adapters use a technology prefix, such as
`Json`, `File`, `PlayerPrefs`, `Unity`, or `ScriptableObject`.

Do not use `Manager`, `Helper`, `Utility`, `Processor`, or `Handler` when a standard responsibility name is available.
Event callbacks use the `On...` prefix; `Handle...` remains reserved for non-event command or workflow processing.

## Data representation boundaries

Keep the following representations separate:

1. `Config` and `Entry` types optimize authoring and Unity serialization.
2. `Document` and `SaveData` types define stable external serialization contracts and versions.
3. `Snapshot`, `Definition`, and `Catalog` types express immutable runtime meaning and enforce invariants.
4. `State` types represent mutable runtime behavior.

Loaders and mappers cross these boundaries. Domain and state code must not read JSON, files, PlayerPrefs, or
`ScriptableObject` fields directly.

A whole-data load should produce one aggregate snapshot and register that snapshot once. Consumers receive the
snapshot or its owned parts, never the concrete source adapter.

## Authored asset placement

- Keep project-authored configuration assets under `Assets/_Project/Configuration`.
- Organize configuration assets by their owning runtime area first, for example `Game`, `Presentation`, `UI`, or
  `User`.
- Keep the C# configuration types beside the subsystem whose data they author; do not mirror the asset folder by
  centralizing unrelated configuration code.
- Keep framework, package, and application assembly settings under `Assets/_Project/Settings`, grouped by the
  capability or integration they configure.
- When an integration loads settings through `Resources`, place its required `Resources` folder below the owning
  settings capability and integration so the expected resource key remains unchanged.
- Do not mix framework settings with authored game, presentation, UI, or user data merely because both use
  `ScriptableObject`.
- Keep shared Unity swatch and preset libraries under `Assets/_Project/Presets`.
- Place Unity project preset-library files in a nested `Editor` folder so Unity discovers them without mixing them
  with the editor-tooling source assembly.

## Dependency rules

The required direction is:

```text
Composition -> UI / Presentation / User / Game / Infrastructure / Validation
Editor      -> runtime assemblies
UI          -> Presentation / User / Game
Presentation-> User / Game / generic UI primitives
User        -> Game value objects / Infrastructure / Validation
Game services -> Game core / focused User state contracts
Game adapters  -> Game core / Infrastructure / Validation
Game config -> Game / Infrastructure / Validation / Unity authoring APIs
Game core   -> stable framework APIs and generic Validation only
Infrastructure and Validation -> stable framework APIs
```

Avoid bidirectional feature dependencies. If two features need the same concept, move that concept to the feature
that semantically owns it or introduce a small contract at the consumer-facing boundary.

## Namespace and file rules

- Namespace segments mirror folders inside their containing assembly.
- A file is named after its primary type.
- Prefer one primary type per file. Small DTOs or entries may share a file only when they form one inseparable
  serialized contract and are not reused independently.
- Keep configuration types with the subsystem whose data they author or construct.
- Moving a serialized Unity type requires preserving its `.meta` GUID and evaluating whether `MovedFrom` or
  `FormerlySerializedAs` is needed.
- Do not introduce a new top-level area or architectural role folder without updating this document.

## Review checklist

For every new or moved type, verify:

- The path identifies one clear owner and feature.
- The folder name has a defined meaning in this document.
- The namespace mirrors the path.
- The suffix matches the type's actual responsibility.
- Dependencies point in an allowed direction.
- Domain code does not know its persistence or authoring format.
- Configuration remains beside its owning subsystem.
- Composition contains wiring only and uses only the documented `Scopes`, `Installers`, and `Factories` roles.
- No vague catch-all folder or type name was introduced.
- External representations are versioned where compatibility matters.
- Unity asset GUIDs and serialized references remain valid after moves.

If a type cannot be placed confidently using these rules, pause and resolve its ownership before adding a new folder.

Run the structural architecture check after adding or moving code:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validate-Architecture.ps1
```

The script checks the mechanically enforceable subset of this standard. The semantic ownership and dependency
questions in the checklist still require review.
