# Project Architecture and Naming Standard

This document is the source of truth for intended project-wide placement and naming conventions. Approved production
code and serialized assets are the source of truth for current behavior and actual placement. Keep this standard,
the implementation, and its validators synchronized after an architectural decision.

For a concise map of runtime areas, lifetime boundaries, and core flows, see
[`LL-Runtime-Overview.md`](../Architecture/LL-Runtime-Overview.md).
For a detailed walkthrough of the game-data and user-data pipelines, see
[`Game-And-User-Data.md`](../Architecture/Game-And-User-Data.md).
For C# syntax and lifecycle conventions, see [`Code-Style.md`](Code-Style.md). For validation semantics, automated
coverage, and required verification, see [`Validation.md`](Validation.md).

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
| `Game` | Game rules and reference data | Cards, flags, heroes, items, payments, rank-up, quests, ranks, rewards, upgrades |
| `Infrastructure` | Reusable technical mechanisms | Collection snapshots, compilation and loading contracts, serialization, storage, reporting |
| `Presentation` | Display meaning without visual lifecycle | Flags, hero portraits, item icons, localization, formatting, text tokens, confirmations |
| `UI` | Concrete visual lifecycle and navigation | Controls, graphics, views, visual states, windows, UI flows |
| `User` | User defaults, persistence, snapshots, and live state | Identity, hero selection, inventory, rank progress, rank-up quest |
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
- UI may consume read-only user-state interfaces for display and queries. Only game application services may consume
  `*Commands` interfaces and mutate user state; UI actions must cross a game service boundary.
- `Game/RankUp` owns the rules for advancing to the next rank: its quest path, instant-payment path, and reward.
  `RankUpDefinition` is one immutable rule set, `RankUpQuest` describes its quest path, and `RankUpService` executes
  the selected path. `RankUpQuestService` is the command boundary for starting, completing, expiring, and clearing
  the persisted user quest; UI may observe quest state but must issue mutations through this service. Within this
  feature, use `Definition` for APIs and variables that expose the immutable rule set, and `RankUp` for commands and
  capability names that describe the player action. Use `RankUp`, never `Promotion`, as the code and folder vocabulary
  for this feature.
- `Game/Heroes` owns hero identity and immutable hero definitions. A hero definition contains its localized-name key
  and `FlagId`, while rank-up and other game capabilities refer to it only through `HeroId`.
- `Game/Flags` owns the semantic identity of flags. A `FlagId` names the represented flag directly and must not be
  constrained to a country-code standard because flags may represent fictional countries, factions, or organizations.
- Unity-dependent authoring adapters are allowed under the owning capability's `Configuration` folder.
- Cross-capability workflows belong to the capability that owns the outcome; create a new capability only when no
  existing owner is correct.
- `Game/Data` is reserved for the aggregate game-data loading boundary. It is not a general dumping ground.
- `Game/Data/Declarations` owns the source-neutral, not-yet-trusted aggregate representation produced by every
  game-data source and consumed by `GameDataCompiler`.

### `User`

Owns user-specific defaults, persisted user representation, immutable loaded snapshots, and live mutable user state.

- `Configuration` owns authored defaults and their validation.
- `Defaults` owns the source-neutral defaults pipeline: declarations, source adapters, runtime compilation, and the
  immutable defaults snapshot used to initialize a user without a valid save.
- `Persistence` owns the user repository, session loading and save coordination, document mapping, and version
  handling.
- `Snapshots` owns immutable point-in-time user data.
- `State` owns long-lived mutable runtime state, grouped by capability.
- Each mutable state capability exposes a read interface separately from its `*Commands` interface. Register both
  against one concrete singleton so consumers receive the same state instance with compile-time-appropriate access.
- `User` may depend on stable `Game` value objects and definitions.

### `Infrastructure`

Owns reusable technical mechanisms such as loading contracts, serialization, storage, and platform adapters.

- Infrastructure must not encode game, user, presentation, or UI policy.
- Infrastructure must not depend on `Game`, `User`, `Presentation`, `UI`, `Composition`, or `Editor`.
- `Infrastructure/Loading` owns the generic `IDataSource<TDeclaration>`, `IDataLoader<TData>`, and compiled-loading
  chain. Domain sources implement the generic source contract, while declarations and compilers remain with their
  domain owner.
- `Infrastructure/Collections` owns generic collection-copying helpers. These helpers may define snapshot mechanics
  such as defensive copying, but they must not define domain collection policy or validation.
- Prefer capability folders such as `Saving/Serialization` and `Saving/Storage` over technology-only folders.
- Read-only storage and document-loading contracts remain separate from writable save contracts. A static-data
  source consumes only read capabilities; repositories that persist mutable state consume the writable extension.
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
- `Presentation/Inspector` owns reusable declarative inspector attributes for presentation configuration. These
  attributes compose authoring metadata only; they do not contain editor workflows, asset-database access, or
  runtime presentation behavior.
- `Presentation/Sprites` owns reusable runtime sprite catalogs. `SpriteCatalog<TId>` maps one sprite to a domain ID;
  `SpriteVariantCatalog<TId, TVariant>` maps multiple explicitly named variants to each domain ID.
- `Presentation/Items` owns item formatting and item icon sprites keyed by `ItemId`.
- `Presentation/Heroes` owns the independently loaded small and large portrait resources keyed by `HeroId`.
  Portrait data remains static sprites; optional animation is view behavior and does not change hero definitions.
- `Presentation/Flags` owns flag sprites keyed by `FlagId`. Game hero definitions reference only the semantic ID and
  never reference Unity sprites directly.

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
- Validation vocabulary, behavior, and verification rules are defined in [`Validation.md`](Validation.md).

### `Editor`

Owns Unity Editor-only tooling, menus, inspectors, build checks, and asset-database integration.

- Runtime code must never depend on `Editor`.
- Mirror a runtime capability below `Editor` when the tool is feature-specific.
- Keep project-wide editor workflows in a role folder such as `Menu`, `Creation`, or `Validation`.
- `Toolbar` contains compact controls for frequently used project workflows in Unity's main or window toolbars.
  Unity-version-specific toolbar integration stays isolated here and delegates behavior to editor workflow commands.
- `CI` contains provider-independent batch-mode entry points for validation, tests, and player builds. Provider
  configuration calls these entry points and must not duplicate their project rules.

## Reserved architectural role folders

The following names have project-wide architectural meaning and must be used consistently:

- `Configuration`: Unity-authored or otherwise author-authored input for one owning capability.
- `Declarations`: source-neutral, not-yet-trusted aggregate input normalized from multiple concrete sources.
- `Persistence`: durable/external serialized representation, version handling, mapping, and save/load orchestration.
- `Documents`: versioned external serialized contracts owned by a `Persistence` boundary.
- `Snapshots`: immutable point-in-time representation composed at a loading boundary.
- `State`: live mutable runtime state.
- `Sources`: source-specific adapters that read external or authored data into a source-neutral declaration.
- `Loading`: adapters and orchestration that produce one fully constructed runtime resource or aggregate.
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

This is not a closed list of every legal folder. A feature-local folder such as `Effects`, `Modal`, `Progress`, or
`Values` is valid when its owner and responsibility are clear and it does not redefine a reserved role. Avoid vague
folders such as `Common`, `Misc`, `Helpers`, `Managers`, `Runtime`, or `Data` without a narrowly documented meaning.
A broadly reusable type still needs a concrete owner and responsibility.

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

Compilers implement `IDataCompiler<TDeclaration, TSnapshot>`. Their source-neutral declaration is the only input to
`Compile`; stable catalogs or other services required to resolve it are constructor dependencies.

For game and user domain data, use `IDataLoader<T>` only for an application-level aggregate such as
`GameDataSnapshot`, `UserDefaultsSnapshot`, or `UserSnapshot`. Do not implement it on individual domain catalog
configs or other partial domain data. A whole-data load registers one aggregate once; consumers receive the
aggregate or its owned parts, never a concrete source, repository, or compiler. Presentation and UI resource
catalogs may retain focused loaders because they are independently owned runtime resources rather than partial
domain aggregates. A `Config` remains an authoring and validation object; a technology-specific loader such as
`ScriptableObjectItemIconCatalogLoader` or `ScriptableObjectWindowCatalogLoader` validates that config and constructs
the runtime catalog.

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
- Expose every project-authored root configuration declared with `CreateAssetMenu` through `Last Level/Content`, using
  `ProjectAssetSelector` so the command selects the existing asset or starts its normal creation flow.
- Group `Last Level/Content` entries by the configuration's owning runtime area and separate the groups with menu
  priority gaps. Keep presentation image catalogs, including icons, flags, and portraits, together in their own
  Presentation group, separate from Game, UI, and User content.
- Add the Content menu entry and its automated coverage in the same change as a new root authored configuration.
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

The application uses one process-wide project scope and scene-owned child scopes. A scene scope adds objects whose
lifetime and serialized references belong to that scene; it must not recreate the project scope. The current scope
inventory and registrations are documented in
[`LL-Runtime-Overview.md`](../Architecture/LL-Runtime-Overview.md).

A scope keeps small scene-local registrations visible and constructs larger installers manually with `new`.
Installers register supplied policies but do not select storage, serialization, authoring, or loading technologies.
Factories make those concrete construction choices without performing DI registration. Changing a runtime data
source is therefore a composition-root change rather than an installer or consumer change.

## Current runtime documentation

Current runtime flows, concrete participants, and document versions are descriptive implementation information. Keep
them synchronized in [`LL-Runtime-Overview.md`](../Architecture/LL-Runtime-Overview.md)
and [`Game-And-User-Data.md`](../Architecture/Game-And-User-Data.md). The
normative representation boundaries that those flows must follow remain in this document.

## Dependency rules

The top-level project dependency matrix is:

| From | May depend on |
| --- | --- |
| `Composition` | Every runtime area |
| `Bootstrap` | `UI.Controls` and stable framework APIs |
| `UI` | `Presentation`, `User`, `Game`; `UI.Windows.Configuration` and `UI.Windows.Loading` may also use `Infrastructure` and `Validation` |
| `Presentation` | `Game`, `User`; capability-owned `Configuration` and `Loading` adapters may also use `Infrastructure` and `Validation` |
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

## Related conventions

C# syntax, declaration layout, DI marking, reactive lifecycle, and Unity component conventions are defined only in
[`Code-Style.md`](Code-Style.md). Validation vocabulary, automated coverage, and required change verification are
defined only in [`Validation.md`](Validation.md).

## Changing the architecture

An architectural change is any new top-level area, role folder, dependency direction, assembly, lifetime boundary,
type suffix, or project-wide naming convention. Make such a change in this order:

1. Identify the semantic owner and the consumers.
2. Update this document with the new boundary and allowed direction.
3. Move or add code while preserving Unity GUIDs and serialized keys.
4. Extend `tools/Validate-Architecture.ps1` for every mechanically enforceable part.
5. Update the runtime overview when the change affects its modules, lifetime boundaries, or core flows.
6. Run the checks required by the change matrix in `Validation.md`.

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
questions in the checklist still require review. Automated-coverage classifications and additional checks are
defined in [`Validation.md`](Validation.md).
