# Validation Standard

This document is the source of truth for validation semantics, validator ownership, automated convention coverage,
and required project checks. Architecture rules belong to [`ARCHITECTURE.md`](ARCHITECTURE.md), C# conventions to
[`CODE_STYLE.md`](CODE_STYLE.md), and command details to [`CI.md`](CI.md).

Validators enforce approved production, asset, and agreement decisions; they do not define those decisions. Do not
relocate production code or assets solely because a stale validator expects another layout. Resolve the intended
design first, then update the applicable agreement, validator, discovery path, and tests together.

## Runtime validation vocabulary

Generic validation primitives belong in `Assets/_Project/Scripts/Validation`.

- `ValidationChecks` owns stateless, context-free predicates and simple collection checks. It does not report issues,
  throw because a checked value is invalid, or depend on a project domain.
- Complementary predicates such as `IsPositive` and `IsNonPositive` may provide readable call-site forms, but one
  delegates to the other so the validity formula has one implementation.
- `ValidationRules` adapts checks to `ValidationContext` and owns reusable issue codes and messages. A rule delegates
  its validity decision to the corresponding check rather than duplicating the predicate.
- Configuration validators compose rules to accumulate multiple issues.
- Domain facades such as `IdentifierValidator` and `EnumValidator` may expose `Validate`, `IsValid`, and `EnsureValid`
  over the same underlying checks. They must not reimplement the validity grammar.

Choose an API by intent:

- Use `ValidationChecks` for a shared context-free fact about an input, representation, or invariant.
- Use `ValidationRules` while composing a `ValidationContext`, when failures need stable issue codes, paths,
  messages, or accumulation.
- Use a facade's `Validate` inside an existing validation composition, `IsValid` for an expected non-throwing
  rejection, and `EnsureValid` at a construction or trust boundary that must reject invalid input.
- Use `ValidationRunner` to start or guard a composed validation session, not for normal state transitions or UI
  branching.

## Validation boundaries

- Use a direct guard for a one-off programmer contract, especially a dependency null check or a type-local condition
  with no reusable validation meaning.
- Normal behavior is not validation. `Can...` and `Try...` rejection, optional-state detection, UI visibility,
  clamping, loop bounds, and no-op decisions use ordinary control flow and do not create validation issues.
- Reuse a domain facade when validity belongs to a named concept such as an identifier or country code. Do not
  reconstruct that grammar from individual checks at call sites.
- Consumers of an already-valid `Definition`, `Snapshot`, catalog entry, or domain object do not repeat invariants
  guaranteed at construction. Validate only new input or a relationship owned by the consumer.
- Validation of untrusted `Config`, `Document`, and `Declaration` data accumulates issues through
  `ValidationContext`. Constructor and method guards protect programmer-facing runtime contracts and are not a
  second validation pipeline.
- Validation reports errors and does not silently repair source data.
- Validation for one configuration type stays beside that configuration. Cross-asset or project-wide validation
  using `AssetDatabase` belongs in `Assets/_Project/Editor/Validation`.

## Automated convention coverage

Coverage labels mean:

- **Automated**: the repository check is intended to reject every violation of the stated rule.
- **Partial**: automation detects common violations, but review is still required.
- **Review**: design intent cannot be established reliably by a structural check.

| Agreement area | Coverage | Repository check |
| --- | --- | --- |
| Runtime top-level folders and Composition roles | Automated | `tools/Validate-Architecture.ps1` |
| Namespace-to-folder correspondence | Automated | `tools/Validate-Architecture.ps1` |
| Top-level runtime dependency directions | Partial | `tools/Validate-Architecture.ps1` checks `using LL...` directives |
| Selected DI, data-boundary, and user-state dependencies | Partial | `tools/Validate-Architecture.ps1` |
| Ownership, abstraction value, and feature placement | Review | Architecture review |
| Enum representation and hard line length | Automated | `tools/Validate-CodeStyle.ps1` |
| Attribute layout, empty bodies, and simple expression wrapping | Partial | `tools/Validate-CodeStyle.ps1` |
| DI constructor marking, R3 lifetime, and component multiplicity | Partial | `tools/Validate-CodeStyle.ps1` |
| Naming clarity, member order, null semantics, and lifecycle intent | Review | Code review |
| Unity serialized keys, GUIDs, asset references, and data validity | Partial | Unity validation plus targeted search and review |

When adding or materially changing a structural rule, add focused positive and negative fixtures for the validator
whenever the rule depends on parsing or regular expressions. A validator change is incomplete when its discovery
scope or behavior can regress without a failing test.

## Change verification matrix

Run every row that applies to a change:

| Change | Required verification |
| --- | --- |
| Project-owned C# | Code-style validation and compilation |
| Path, namespace, type role, or dependency | Architecture validation, code-style validation, and compilation |
| New or moved runtime/editor code | Architecture validation and relevant tests |
| DI registration or lifecycle | Architecture validation, code-style validation, compilation, and relevant tests |
| Serialized field or Unity type identity | Explicit asset migration, old-key search, Unity validation, and reference verification |
| Config, document, declaration, snapshot, identifier, or domain invariant | Unity validation and focused compiler, mapper, or domain tests |
| Validator or validation discovery | Positive and negative validator fixtures plus the affected validation command |
| Build scenes, player settings, or platform configuration | Full validation and a relevant player build |
| Architecture boundary or convention | Agreements, overview documents, validators, and validator tests updated together |

Use the narrow checks while iterating and the combined validation command before handing off a change that touches
multiple rows:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/tests/Test-ConventionValidators.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/ci/Validate.ps1
```

## Exceptions

An exception to a project-wide rule must be:

1. Narrowly scoped to a named folder, type, asset, or integration.
2. Documented beside the rule it qualifies, with the reason the general rule does not apply.
3. Reflected in automation without weakening unrelated cases.
4. Covered by a test when the exception affects validator behavior.
5. Removed when its stated reason no longer exists.
