# Domain Concern Conventions

This guide applies to new or substantially extended Domain concerns. It does
not require refactoring existing concerns or creating files that a concern
does not need.

## Start with the contract

Define the public interface and its domain invariants first. Add an abstract
base class only when it can provide shared, concrete behavior. Keep identity,
persistence, transport, and application workflow decisions in their own
concerns and layers.

Keep related files together and add only the files that have a responsibility:

| File | Responsibility |
| --- | --- |
| `<Concern>.Interface.cs` | Public domain contract |
| `<Concern>.Constant.cs` | Justified constraints and stable patterns |
| `<Concern>.Result.cs` | Named success/failure factories using Kernel types |
| `<Concern>.Validator.cs` | Reusable `Result<TValue>` validation |
| `<Concern>.Extension.cs` | Validated operations composed from Result flows |

## Validate through Kernel Result

Use `BuildingBlock.Kernel.Results.Result<TValue>` and
`BuildingBlock.Kernel.Errors.Error`; do not introduce parallel result, error,
or fluent-validation frameworks. Put stable constraints in `Constant`, and
expose named `Error.UnprocessableEntity(code, message)` values under
`<Concern>Result.Failure`.

Validators should return the original value in a failed or successful
`Result<TValue>`, composing checks with `Ensure`. For example, the
Referenceable concern validates before returning its entity:

```csharp
return Result<TValue>.Success(auditable)
    .Ensure(
        predicate: _ => !string.IsNullOrWhiteSpace(reference),
        errorValue: ReferenceableResult.Failure.ReferenceRequired);
```

The named failure is created with `Error.UnprocessableEntity` in
`ReferenceableResult`.

Use precise argument exceptions for constructor invariants. Use Result-based
validation when callers need to handle recoverable boundary or operation
failures. Share the same constraints between both paths rather than silently
substituting valid-looking values.

## Validate before mutating

Extension operations should validate before performing their mutation.
Compose the validator with `Bind`, and mutate only in `Tap` after validation
succeeds:

```csharp
return result
    .Bind(entity => ReferenceableValidator.ValidateReference(entity, reference))
    .Tap(entity => entity.Reference = reference!);
```

Because `Bind` and `Tap` continue only on success, a validation failure leaves
the entity unchanged. Follow the established
[Referenceable validator](../../src/BuildingBlocks/BuildingBlock.Domain/Concerns/Foundation/Referenceable/Referenceable.Validator.cs)
and
[Referenceable extension](../../src/BuildingBlocks/BuildingBlock.Domain/Concerns/Foundation/Referenceable/Referenceable.Extension.cs)
for a concrete example using the repository's Kernel APIs.

## Keep the design proportional

- Do not create empty convention files or force every concern to use every
  file type.
- Add limits only when a domain or concrete boundary requirement justifies
  them; avoid speculative maximum lengths and counts.
- Do not add validation packages, persistence, transport, or infrastructure
  abstractions without a concrete use case.
- Keep mutation behavior explicit and keep unrelated concerns unchanged.
