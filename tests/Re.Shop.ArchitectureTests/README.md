# Re.Shop.ArchitectureTests

Spec §5.3 dependency rules asserted against compiled assemblies and project files. _test · internal_

## Summary

This project is the executable form of Re.Shop's Clean Architecture rules: seven
NetArchTest rules over compiled assemblies, three assertions parsed from `.csproj`
files, and seven anchor tests pinning each marker to its assembly. It references
every other project precisely so it can inspect them, and it is the only test
project allowed to reach across all five `src/` layers. If a rule fails, fix the
`ProjectReference` — never weaken the rule.

## Contains

| Path | Role | Notes |
|---|---|---|
| `Marker.Spec.cs` | Anchor tests | 7 facts: each `*Marker` resolves to its own assembly |
| `Dependency.Spec.cs` | Assembly rules | 7 facts covering spec §5.3 rules 1–5 (NetArchTest) |
| `ProjectFile.Spec.cs` | Project-file rules | 3 facts covering rules 6–8, parsed as XML |
| `Re.Shop.ArchitectureTests.csproj` | Project file | References all 5 `src/` + `app/api` + `app/admin` |

## Boundaries

In scope:
- Dependency direction between compiled assemblies.
- `OutputType` and `ProjectReference` entries in `.csproj` files.

Out of scope:
- Behavioural correctness of a layer → `../Re.Shop.SharedKernel.UnitTests/` and siblings.
- Whether the API works end-to-end → `../Re.Shop.IntegrationTests/`.
- Rules 7–8 cannot be assembly-level checks: the AppHost compiles its references
  out (`ReferenceOutputAssembly=false`), so they parse the project file instead.

## Assumptions

- `src/` and `app/` keep their `*Marker` type as an assembly anchor. Domain
  entity/aggregate types are validated by Domain unit specs; `ShopDbContext`
  does not exist yet.
- `RepoRoot` is located by walking up from the test output until `Re.Shop.slnx`
  is found.
- NetArchTest.Rules 1.3.2, Shouldly 4.3.0 and xunit v3 are pinned centrally in
  the root `Directory.Packages.props`; this project declares no versions.

## Invariants

- Spec §5.3 rules 1–8 hold at all times; a failing rule is fixed by correcting
  the `ProjectReference`, never by weakening the assertion.
- Each `*Marker` type resolves to the assembly that declares it.

## Conventions

- File suffix is `.Spec.cs` — a unit-level spec, not an end-to-end test.
- NetArchTest's `.Should().NotHaveDependencyOn(...)` and Shouldly's
  `.ShouldBe(...)` are different libraries; do not rewrite one as the other.
- A failing rule must name the offending type in its message.

## Related

- Parent: [`../README.md`](../README.md)
- Rules: [`../../docs/superpowers/specs/2026-10-01-central-config-clean-architecture-scaffold-design.md`](../../docs/superpowers/specs/2026-10-01-central-config-clean-architecture-scaffold-design.md)
- Naming: [`../../guide/dotted-file-naming-guide.md`](../../guide/dotted-file-naming-guide.md)

## Notes

- Domain now contains value objects, entity/aggregate bases, and event contracts,
  so the project-reference rules apply to real model code. Other source layers
  still primarily contain their assembly markers.
- All eight §5.3 rules are covered; the original plan's Task 10 had nine methods
  and missed rule 6.
