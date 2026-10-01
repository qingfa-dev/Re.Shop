# Dotted File Naming — Style Guide

A convention for naming files so the name itself says two things: *what the
file is about* and *what role it plays* — drawn from a small, shared
vocabulary, never free text.

```
user.service.ts              # TS/JS: subject.qualifier.ext
Order.Interface.cs           # C#/.NET: Concept.Responsibility.cs
```

Two ecosystems, two casings, same underlying idea. Pick the track that
matches the project:

- **Track A** — TypeScript / JavaScript, and config/infra files
- **Track B** — .NET / C#

---

## Track A — TypeScript / JavaScript

### The pattern
```
<subject>.<qualifier>[.<qualifier2>].<ext>
```
- **subject** — the domain concept (`user`, `order`, `auth`). Lowercase,
  kebab-case if multi-word (`user-profile`).
- **qualifier** — the file's *role*, from the table below. One is normal; a
  second is added only for a genuine sub-role (`user.service.spec.ts`).

### Common qualifiers
| Layer | Qualifiers |
|---|---|
| API / web layer | `controller` `service` `guard` `middleware` `interceptor` `resolver` `router` |
| Data layer | `model` `entity` `repository` `dto` `schema` `migration` `seed` |
| Types & contracts | `interface` `type` `enum` `constant` `config` |
| UI | `component` `directive` `pipe` `hook` `store` `slice` `page` `layout` |
| Logic helpers | `util` `helper` `factory` `mapper` `validator` `decorator` |
| Testing | `spec` `test` `mock` `fixture` `stub` |

Stick to qualifiers the team already uses. Adding a new one is a small
convention change — worth a deliberate decision, not an ad hoc choice on one
file.

### Stacking qualifiers
Broadest role first, sub-role second:
```
user.service.spec.ts        # the service, and specifically its test
order.controller.e2e-spec.ts
auth.guard.mock.ts
```

### Casing
- Lowercase throughout; hyphenate multi-word subjects
  (`user-profile.service.ts`, not `UserProfile.service.ts` or
  `user_profile.service.ts`).
- Acronyms stay lowercase (`api-client.service.ts`, not
  `API-Client.service.ts`).
- Never mix casing styles within one filename.

### Folder naming
Dotted files live in kebab-case feature folders that mirror the subject:
```
src/
  user/
    user.controller.ts
    user.service.ts
    user.repository.ts
    user.service.spec.ts
```
Don't repeat the folder as a redundant qualifier (`user/user.user.ts`), and
don't let folder and file disagree on singular/plural (`users/user.service.ts`
is inconsistent — pick one per project).

---

## Track B — .NET / C#

Same underlying idea, `Concept.Responsibility.cs`, but PascalCase, and the
folder path *is* the namespace (the class name is already the filename in
.NET, so the "subject" lives in both places at once).

### The pattern
```
<TermPlural>/<Concept>/<Concept>.cs
<TermPlural>/<Concept>/<Concept>.<Responsibility>.cs
```
```
Orders/Order/Order.cs
Orders/Order/Order.Interface.cs
Orders/Order/Order.Result.cs
```
- Outer folder is **plural** (`Orders`); inner concept folder is **singular**
  and matches the type name (`Order`).
- The namespace mirrors the folder path exactly:
  `BuildingBlocks.Core.<Area>.Orders.Order` (adjust the root per project).

### The Responsibility vocabulary — core five
Deliberately short. Each suffix names a recognized architectural role, not
a description of what one file happens to do.

| Suffix | Responsibility |
|---|---|
| *(none)* — plain `Concept.cs` | Single concrete type, single responsibility (e.g. `LocalizedValue.cs`) |
| `.Interface.cs` | Contract(s) for the concept |
| `.Base.cs` | Abstract base implementation |
| `.Extension.cs` | Extension methods / combinators |
| `.Result.cs` | Outcome/error-code holder — `<Concept>Result`, with error codes as `<Subject>.<Field>.<Rule>` |
| `.Constant.cs` | Shared constants — only when values are non-trivial enough to need a home |

A suffix earns a spot here only if it names a GoF pattern, a DDD building
block, or a standard .NET framework convention. A longer, still-curated
extended list (`.Factory`, `.Builder`, `.Options`, `.Validator`,
`.Specification`, and a handful more) lives in the skill's reference file,
for when the core five genuinely don't fit — and even that list is closed,
not a template for inventing new ones.

### Sub-suffixes
Allowed for a real narrower role, stacked broad-first, added only once the
file has actually grown to need the split — never pre-emptively:
```
Order.Result.Errors.cs      # error-code set outgrew the nested class
Order.Extension.Async.cs    # async extension methods split from the sync set
```
Reaching for a third level of suffix is usually a sign the *concept* needs
splitting, not that the filename needs to get longer.

### Error codes
`<Subject>.<Field>.<Rule>`, consistently, everywhere in the repo:
```
Order.ShippingAddress.Required
Order.Total.Positive
```

---

## Where dotted naming doesn't apply

- **Python** — a dot in a filename signals a package boundary to the import
  system, so Python uses `snake_case` throughout (`user_service.py`), never
  dotted qualifiers.
- **Java / Kotlin** (non-.NET JVM) — the class name is the filename
  (`UserService.java`); the package path carries the "dots," and there's no
  suffix convention layered on top the way Track B has for .NET.
- **React components** — often `PascalCase.tsx` with no qualifier
  (`Button.tsx`) rather than `button.component.tsx`. Check which the
  project already does; don't mix the two in one codebase.

Config and infra files borrow Track A's pattern even outside JS/TS —
`docker-compose.override.yml`, `jest.config.ts`, `webpack.config.prod.js` —
because the qualifier there means "environment/variant," the same idea as a
sub-role.

---

## Anti-patterns

**Free-text qualifiers** (Track A) — defeats the point of a fixed vocabulary.
```
# Bad                          # Good
user.does-the-auth-stuff.ts    user.guard.ts
```

**Invented suffix instead of the vocabulary** (Track B).
```
# Bad                  # Good
Order.HandlesStuff.cs  Order.Interface.cs
```

**Redundant subject/concept** — restating what the folder already says.
```
# Bad (inside src/user/)              # Good
user/user-related-service.service.ts  user/user.service.ts
```

**Inconsistent casing within one project.**
```
# Bad — two styles, same project     # Good — pick one
userProfile.service.ts               user-profile.service.ts
order-item.service.ts                order-item.service.ts
```

**Qualifier/suffix order that reads backwards.**
```
# Bad                  # Good
user.spec.service.ts   user.service.spec.ts
```

**Namespace that doesn't mirror the folder** (Track B).
```
# Bad — folder is Orders/Order/, namespace is flattened
namespace BuildingBlocks.Core.OrderManagement;

# Good — mirrors the folder path exactly
namespace BuildingBlocks.Core.Sales.Orders.Order;
```

**Error codes that skip the three-part shape** (Track B).
```
# Bad                    # Good
"OrderShippingInvalid"   "Order.ShippingAddress.Required"
```

---

## Best practices

1. Keep the vocabulary short and shared — write it down once (this file, or
   a `CONTRIBUTING.md` section).
2. One qualifier/suffix for most files; a second only for a genuine
   sub-role, and only once the file has actually grown to need it.
3. Subject/Concept names the domain concept, not the implementation detail.
4. Casing is consistent within an ecosystem — lowercase kebab-case for
   TS/JS, PascalCase for C# — never mixed within one project.
5. Folder structure and file subject/concept agree, not duplicate each
   other; in C#, the namespace mirrors the folder path exactly.
6. Before introducing a new qualifier or Responsibility suffix, check the
   core table and the extended reference first. It should name a recognized
   role, not describe what one file happens to do.
