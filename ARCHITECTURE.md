# Project Architecture and Naming Standard

This document is the source of truth for intended project-wide placement and naming conventions. Approved production
code and serialized assets are the source of truth for current behavior and actual placement. Keep this standard,
the implementation, and its validators synchronized after an architectural decision.

For a concise map of runtime areas, lifetime boundaries, and core flows, see
[`Documentation/Architecture/LL-Runtime-Overview.md`](Documentation/Architecture/LL-Runtime-Overview.md).
For a detailed walkthrough of the game-data and user-data pipelines, see
[`Documentation/Architecture/Game-And-User-Data.md`](Documentation/Architecture/Game-And-User-Data.md).

The rules describe the intended architecture, not merely the current directory tree. New code must follow them.
When existing code is changed substantially, move it toward this standard when that can be done safely within the
task scope.

## Scope and enforcement

The project-owned runtime code is the `LL.Runtime` assembly rooted at `Assets/_Project/Scripts`. Its top-level
folders are logical modules inside one physical assembly. The single assembly keeps iteration and Unity integration
simple, but it does not enforce module boundaries by itself; `tools/Validate-Architecture.ps1` therefore treats
paths, namespaces, and `using LL...` directives as enforceable architecture.

Split a logical module into another runtime assembly only when at least one of these conditions is true:

- Unity must compile or load the module independently.
- A platform or optional-package boundary requires separate references.
- The dependency boundary is stable enough that assembly-level enforcement is worth the added compilation and
  integration cost.
- A separately testable pure-C# core can avoid Unity references without duplicating contracts.

Do not create an assembly solely to mirror a folder. A new assembly requires an explicit dependency update in this
document, an `.asmdef`, tests for its boundary, and an update to the runtime overview.

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

## Runtime module map

`LL.Runtime` contains exactly these top-level areas:

| Area | Owns | Representative capabilities |
| --- | --- | --- |
| `Bootstrap` | First-scene startup coordination | Localization readiness, minimum display time, scene activation |
| `Composition` | Object graph and lifetime wiring | Project, bootstrap, and main scopes; installers; factories |
| `Game` | Game rules and reference data | Cards, items, payments, rank-up, quests, ranks, rewards, upgrades |
| `Infrastructure` | Reusable technical adapters | Loading, serialization, save services, storage, console reporting |
| `Presentation` | Display meaning without visual lifecycle | Icons, localization, display formatting, text tokens, confirmations |
| `UI` | Concrete visual lifecycle and navigation | Controls, graphics, views, visual states, windows, UI flows |
| `User` | User defaults, persistence, snapshots, and live state | Identity, inventory, rank progress, rank-up quest |
| `Validation` | Reusable validation vocabulary | Contexts, issues, results, rules, reporting contracts |

`AssemblyInfo.cs` and `LL.Runtime.asmdef` are the only files allowed directly at the runtime root. Adding another
top-level area is an architecture change, not a local folder choice.

## Top-level ownership

### `Composition`

Owns application assembly: concrete implementation selection, manual object construction, DI registrations, and
lifetime configuration.

- `Scopes` contains VContainer lifetime scopes. A scope stores serialized Unity references, defines a lifetime
  boundary, performs small scene-local registrations, and orchestrates larger installers.
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
- A scope may register its serialized scene components and a small cohesive set of scene-local services or entry
  points directly when keeping the registrations visible makes the scene composition easier to understand.
- Move registrations to an installer when they form a named subsystem, are reused by more than one scope, require
  their own construction policy, or change independently from the scene boundary. Do not create an installer merely
  to shorten a scope.
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
- `Game/RankUp` owns the rules for advancing to the next rank: its quest path, instant-payment path, and reward.
  `RankUpDefinition` is one immutable rule set, `RankUpQuest` describes its quest path, and `RankUpService` executes
  the selected path. Within this feature, use `Definition` for APIs and variables that expose the immutable rule set,
  and `RankUp` for commands and capability names that describe the player action. Use `RankUp`, never `Promotion`, as
  the code and folder vocabulary for this feature.
- Unity-dependent authoring adapters are allowed under the owning capability's `Configuration` folder.
- Cross-capability workflows belong to the capability that owns the outcome; create a new capability only when no
  existing owner is correct.
- `Game/Data` is reserved for the aggregate game-data loading boundary. It is not a general dumping ground.
- `Game/Data/Declarations` owns the source-neutral, not-yet-trusted aggregate representation produced by every
  game-data source and consumed by `GameDataCompiler`.

### `User`

Owns user-specific defaults, persisted user representation, immutable loaded snapshots, and live mutable user state.

- `Configuration` owns authored defaults and their validation.
- `Persistence` owns the user repository, session loading and save coordination, document mapping, and version
  handling.
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

- Presentation may depend on `Game`, `User`, `Infrastructure.Loading`, and `Validation`.
- `Typography` owns pure rich-text tokens, tags, symbols, styles, and formatting. These types produce display strings
  and must not depend on TMPro, Unity components, windows, or concrete views.
- Confirmation interfaces and their localization keys belong to the presentation capability that defines the user
  decision. A concrete confirmation implemented with a window belongs to `UI/Windows`.
- Presentation must not own reusable visual controls, concrete feature views, navigation, or game rules.
- Presentation must not depend on `UI`.
- Presentation-specific configuration stays with its presentation capability.

### `UI`

Owns concrete visual behavior: views, windows, controls, graphics, navigation, visual state, and visual flows.

- UI may depend on `Presentation`, `Game`, and `User`.
- `MonoBehaviour`, visual components, and window implementations normally belong here.
- `Controls` owns self-contained reusable UI elements, including both interactive controls and display-only elements
  such as progress bars and formatted labels.
- `Views` owns composed visual blocks that present a specific UI concept and can be embedded in more than one screen.
  Feature-owned views stay with their feature instead of moving to a generic shared folder.
- Specialized rendering and visual-state behavior belongs in role folders such as `Graphics` or `VisualStates`.
- UI consumes `Presentation/Typography`; it does not define a second text-token vocabulary.
- `Localization` owns concrete localized UI components; localization services and presentation wording remain under
  `Presentation`.
- `Windows` owns window definitions, navigation, window flows, and concrete windows. Place each concrete window and
  its feature-specific parts directly under a feature folder such as `Windows/RankUp` or `Windows/Upgrade`.
- Keep only foundational UI mechanisms at the `UI` root. A reusable element with a recognized role must use its role
  folder.
- Feature-specific views stay under their feature or window view hierarchy.
- UI must not contain persistence, storage, or domain rules.

### `Validation`

Owns reusable validation primitives, results, contexts, rules, and reporting contracts.

- Generic validation belongs in runtime `Scripts/Validation`.
- Validation of one configuration type stays beside that configuration.
- Cross-asset and project-wide validation that uses `AssetDatabase` belongs in `Editor/Validation`.
- Validation reports errors; it must not silently repair source data.
- Validators enforce approved code, asset, and architecture decisions; they do not establish those decisions.
- Do not relocate production code or assets solely because a stale validator expects another layout. Resolve the
  intended design, then update this document and the validator to match it.
- A change to paths, namespaces, serialized shapes, dependencies, data flow, identifiers, or domain invariants must
  review and run the affected validation. Update validation discovery, rules, and tests in the same change.

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
- `Declarations`: source-neutral, not-yet-trusted aggregate input normalized from multiple concrete sources.
- `Persistence`: durable/external serialized representation, version handling, mapping, and save/load orchestration.
- `Documents`: versioned external serialized contracts owned by a `Persistence` boundary.
- `Snapshots`: immutable point-in-time representation composed at a loading boundary.
- `State`: live mutable runtime state.
- `Sources`: source-specific adapters that read external or authored data into a source-neutral declaration.
- `Serialization`: object-to-byte or object-to-text format conversion.
- `Storage`: raw byte or text access to a medium, addressed without domain knowledge.
- `Services`: cohesive domain or application operations with no more specific feature role.
- `Views`: concrete visual representations.
- `Controls`: self-contained reusable interactive or display-only UI elements; only under `UI`.
- `Graphics`: reusable custom rendering components; only under `UI`.
- `Typography`: pure display text vocabulary and formatting; only under `Presentation`.
- `VisualStates`: reusable visual-state sources, effects, and state values; only under `UI`.
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
- `ManifestConfig`: root authoring object that references every configuration required to build one aggregate.
- `Entry`: one serialized authoring row nested under a configuration.
- `DocumentEntry`: one serialized transport row nested under a `Document`.
- `Declaration`: normalized, source-neutral, not-yet-trusted data prepared for compilation into runtime meaning.
- `Definition`: immutable domain description used by game rules.
- `Catalog`: immutable indexed collection of definitions or resources.
- `Snapshot`: immutable, internally consistent point-in-time aggregate.
- `Document`: versioned external serialized contract, including both reference data and persisted user state.
- `Compiler`: validates and resolves an aggregate `Declaration`, then constructs its immutable runtime snapshot.
- `Mapper`: pure conversion between representations; it performs no I/O and owns no state.
- `Loader`: performs one load operation and returns a fully constructed result.
- `Repository`: loads and saves one semantic aggregate without exposing serialization or storage technology.
- `Coordinator`: owns the lifecycle and side effects of a continuing process but contains no domain rules.
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
2. `Document` and `DocumentEntry` types define stable external serialization contracts and versions.
3. `Declaration` types normalize every source into one source-neutral shape without asserting runtime validity.
4. `Compiler` types validate declarations, resolve references, and construct runtime meaning.
5. `Snapshot`, `Definition`, and `Catalog` types express immutable, already-valid runtime meaning.
6. `State` types represent mutable runtime behavior and can produce an aggregate snapshot.

Sources and repositories own I/O. Mappers only convert representations. Compilers own aggregate validation and
reference resolution. Domain and state code must not read JSON, files, PlayerPrefs, or `ScriptableObject` fields
directly.

For game and user domain data, use `IDataLoader<T>` only for an application-level aggregate snapshot such as
`GameDataSnapshot` or `UserSnapshot`. Do not implement it on individual domain catalog configs or other partial
domain data. A whole-data load registers one aggregate snapshot once; consumers receive the snapshot or its owned
parts, never a concrete source, repository, or compiler. Presentation and UI resource catalogs may retain focused
loaders because they are independently owned runtime resources rather than partial domain aggregates.

## Authored asset placement

- Keep project-authored configuration assets under `Assets/_Project/Configuration`.
- Organize configuration assets by their owning runtime area first, for example `Game`, `Presentation`, `UI`, or
  `User`.
- Keep assets directly under their owning runtime area while each feature contributes only one authored asset. Add a
  feature subfolder only when it contains a coherent group of multiple related assets or needs meaningful nested
  structure; do not create one-asset folders solely to mirror the C# hierarchy.
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
- Organize UI prefabs by the same ownership vocabulary as UI code: reusable controls under `Controls`, composed
  display blocks under `Views`, feature-owned views under their feature, and window roots under `Windows`.
- Name prefab assets `P_<SemanticName>`. The prefab root object must match the asset name, while an instance override
  may use a more specific contextual name.
- Prefab names must describe purpose or visual role. Do not use sequence-only variants such as `_01`; use a semantic
  variant such as `Primary`, `Outlined`, or `Timed`, and add dimensions only when they distinguish intentional sizes.
- Use `Layouts` only for reusable components whose responsibility is arranging children. Do not classify a composed
  display block as a layout merely because it contains several visual elements.

## Lifetime and composition model

The application has three composition boundaries:

| Boundary | Lifetime | Responsibilities |
| --- | --- | --- |
| `ProjectLifetimeScope` | Whole process | Window services, presentation, validation, game data, user state, game services |
| `BootstrapLifetimeScope` | Bootstrap scene | Progress view, bootstrap operations, transition to `Main` |
| `MainLifetimeScope` | Main scene | Scene window controller, modal adapters, rank-up flow, upgrade flow |

A scope registers small scene-local composition directly and constructs larger installers manually with `new`.
Installers are not DI services and their constructors do not use `[Inject]`. Runtime services, controllers, flows,
and MonoBehaviour injection methods resolved by VContainer do use `[Inject]`.

The project scope is auto-created from `VContainerSettings`; the bootstrap scene must not create another project
scope. Scene scopes inherit project registrations through the configured parent relationship.

## Runtime data flows

### Game reference data

```text
GameDataManifestConfig -> ScriptableObjectGameDataSource
                       -> GameDataDeclaration
                       -> GameDataCompiler
                       -> GameDataSnapshot
                       -> immutable catalog parts registered once
```

The alternative serialized path is:

```text
ISaveStorage -> ISaveService -> SerializedGameDataSource
             -> GameDataDocument -> GameDataDocumentMapper
             -> GameDataDeclaration -> GameDataCompiler -> GameDataSnapshot
```

Both sources end at the same declaration. A source may check transport shape and document version, but only the
compiler validates aggregate identifiers and references or creates domain definitions and catalogs.

### User state

```text
IUserDefaultsFactory -----\
                          -> UserSessionLoader -> UserSnapshot -> UserState
IUserSaveRepository ------/

UserState.Changed -> UserSaveCoordinator -> IUserSaveRepository
                                         -> UserSaveDocumentMapper
                                         -> UserSaveDocument
```

The defaults factory creates a fresh valid snapshot. The repository owns the persisted user document and reports a
semantic load result; it does not decide whether defaults should replace missing, corrupt, or unsupported data.
`UserSessionLoader` owns that startup policy. `UserState` is the single mutable aggregate, emits one change
notification per successful logical mutation, and creates the complete snapshot saved by `UserSaveCoordinator`.

Snapshots are immutable load-boundary values. State objects are the only long-lived mutable representation. Current
user documents start at version `1`; future version changes require explicit migration rather than compatibility
logic inside the repository or mapper.

### UI navigation and flows

```text
WindowCatalogConfig -> WindowCatalog -> WindowProvider
    -> WindowNavigator -> WindowController -> Window<TParameters>
```

`RankUpFlow` and `UpgradeFlow` coordinate use cases and windows. Game services own rule execution; flows own
sequencing and presentation decisions; windows own visual behavior.

## Dependency rules

The top-level project dependency matrix is:

| From | May depend on |
| --- | --- |
| `Composition` | Every runtime area |
| `Bootstrap` | `UI.Controls` and stable framework APIs |
| `UI` | `Presentation`, `User`, `Game`; `UI.Windows.Configuration` may also use `Infrastructure.Loading` and `Validation` |
| `Presentation` | `Game`, `User`; configuration adapters may also use `Infrastructure.Loading` and `Validation` |
| `User` | `Game`, `Infrastructure`, `Validation` |
| `Game` core | Other `Game` capabilities and generic `Validation` |
| `Game` services | `Game` core and focused `User.State` contracts |
| `Game` configuration/data adapters | `Game` core, `Infrastructure`, and `Validation` |
| `Infrastructure` | Stable framework APIs and generic `Validation` reporting contracts |
| `Validation` | Stable framework APIs |
| `Editor` | Runtime assemblies and Unity Editor APIs |

The matrix is a maximum permission, not a reason to add a dependency. Folder-specific exceptions do not grant the
same dependency to the rest of their top-level area.

The following directions are always forbidden:

- Any runtime area to `Composition` or `Editor`.
- `Presentation` to `UI`.
- `Game` core models, definitions, catalogs, identifiers, and calculations to `User`, `Presentation`, or `UI`.
- `Infrastructure` or `Validation` to game, user, presentation, or UI policy.
- `User` to `Presentation` or `UI`.

Avoid bidirectional feature dependencies. If two features need the same concept, move that concept to the feature
that semantically owns it or introduce a small contract at the consumer-facing boundary.

## Namespace and file rules

- Namespace segments mirror folders inside their containing assembly.
- A file is named after its primary type.
- Prefer one primary type per file. Small DTOs or entries may share a file only when they form one inseparable
  serialized contract and are not reused independently.
- Keep configuration types with the subsystem whose data they author or construct.
- Moving or renaming a serialized Unity type requires preserving its `.meta` GUID and adding `MovedFrom` when Unity
  needs the old assembly, namespace, or type identity.
- Renaming a serialized field requires explicitly migrating its key in every scene, prefab, and asset, then verifying
  that the old key no longer exists. Do not use `FormerlySerializedAs`.
- Do not introduce a new top-level area or architectural role folder without updating this document.

## Code and lifecycle invariants

- Constructors selected by VContainer and injection methods named `Construct` have `[Inject]`. Manually constructed
  installer, factory-product, DTO, snapshot, definition, and value-object constructors do not.
- A `Construct` method only validates and assigns dependencies. Subscription, initialization, UI refresh, and other
  side effects start in the appropriate lifecycle method.
- A MonoBehaviour creates long-lived R3 subscriptions in `Start` and binds them with `AddTo(this)`. Dynamically
  replaced subscriptions have an explicit active lifetime and are still disposed on destruction.
- Event and reactive callbacks use `On...`; `Handle...` is reserved for command or workflow processing.
- A concrete MonoBehaviour has `[DisallowMultipleComponent]` when a second instance on one GameObject has no defined
  behavior. Abstract component bases need not declare it.
- Serialized fields are declared first. Constants follow serialized fields. Other fields, properties, constructors,
  lifecycle methods, public/internal behavior, and private helpers follow in that order when practical.
- Every project enum uses `byte`, assigns explicit sequential values starting at `0`, and preserves existing numeric
  values when serialized or persisted.
- C# attributes occupy separate lines except a serialized field's constraints and decorators, which share its
  `[SerializeField, ...]` list.
- Empty bodies are inline as `{ }`. Treat 120 characters as the point at which line wrapping deserves an explicit
  readability decision: lines from 121 through 140 characters may remain intact when the single-line form is clearer,
  and should be wrapped when the split reads better. Keep simple assignments and expressions on one line when they
  fit within 120 characters; never break immediately after an assignment operator merely to shorten the line. Keep a
  simple invocation with one expression or lambda argument on one line under the same condition. The hard limit is
  140 characters. A C# file ends immediately after its final non-empty line without a trailing CR or LF.

## Changing the architecture

An architectural change is any new top-level area, role folder, dependency direction, assembly, lifetime boundary,
type suffix, or project-wide naming convention. Make such a change in this order:

1. Identify the semantic owner and the consumers.
2. Update this document with the new boundary and allowed direction.
3. Move or add code while preserving Unity GUIDs and serialized keys.
4. Extend `tools/Validate-Architecture.ps1` for every mechanically enforceable part.
5. Update the runtime overview when the change affects its modules, lifetime boundaries, or core flows.
6. Run architecture validation, compile the assemblies, and run the relevant tests.

Exceptions must be narrow, named by folder or type, and documented beside the rule they qualify. Do not weaken a
top-level dependency rule to accommodate one adapter.

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
- Lifetime scopes keep small scene-local composition visible and delegate named subsystems to installers.
- No vague catch-all folder or type name was introduced.
- External representations are versioned where compatibility matters.
- Unity asset GUIDs and serialized references remain valid after moves.
- Validators and their discovery paths reflect the current code, asset layout, data flow, and domain invariants.

If a type cannot be placed confidently using these rules, pause and resolve its ownership before adding a new folder.

Run the structural architecture check after adding or moving code:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Validate-Architecture.ps1
```

The script checks the mechanically enforceable subset of this standard. The semantic ownership and dependency
questions in the checklist still require review.
