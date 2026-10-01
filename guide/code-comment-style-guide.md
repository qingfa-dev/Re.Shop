# Code Comment Labels — Style Guide

A labeling convention that replaces long prose comments with short, greppable
tags. The label is the headline; the rest of the line is the one thing a
reader couldn't infer from the code itself.

## Should this line get a comment at all?

Ask, in order:

1. **Is the "what" obvious from the code?** If yes, no comment.
2. **Is there a "why" — a business rule, external assumption, or non-obvious
   edge case — that isn't visible in the code?** If yes, comment it.
3. **Would a teammate reviewing this in six months ask "wait, why does it do
   that?"** If yes, comment it.

Most lines fail step 1 and need nothing. Reserve labels for the lines that
pass step 2 or 3.

## Quick Reference

| Category | Key Labels | Use When |
|---|---|---|
| Validation | `Validate:` `Check:` `Guard:` `Verify:` `Assert:` | Preconditions, input rules, invariants |
| Objects | `Create:` `Assign:` `Update:` `Initialize:` `Merge:` `Clone:` | Object lifecycle, data manipulation |
| Processing | `Compute:` `Transform:` `Filter:` `Parse:` `Normalize:` `Generate:` | Calculations, formatting, data shaping |
| Flow | `Await:` `Retry:` `Skip:` `Fallback:` `Batch:` `Throttle:` | Async ops, control flow, degradation |
| Resources | `Acquire:` `Release:` `Lock:` `Cache:` `Pool:` `Dispose:` | Connections, memory, cleanup |
| Events | `Raise:` `Trigger:` `Notify:` `Enforce:` `Handle:` | Domain events, business rules |
| Integration | `Call:` `Send:` `Receive:` `Map:` `Publish:` `Webhook:` | External APIs, messaging |
| Errors | `Catch:` `Recover:` `Compensate:` `Escalate:` `Degrade:` | Exception handling, rollback |
| Observability | `Log:` `Trace:` `Monitor:` `Audit:` `Profile:` | Debugging, metrics, compliance |

## How each category is used

### Validation & Checks
Preconditions and conditional guards.
```csharp
// Validate: Email format matches RFC 5322 and domain whitelist
if (!Regex.IsMatch(email, pattern) || !IsAllowedDomain(email))
    throw new InvalidEmailException(email);

// Guard: Array bounds within limits
if (index < 0 || index >= items.Length)
    throw new IndexOutOfRangeException();
```

### Object Operations
Creation, mutation, and combination of objects.
```csharp
// Create: New order with default status
var order = Order.Create(customerId);

// Merge: Customer data from multiple sources, account data wins on conflict
var customer = CustomerMerger.Merge(crmData, accountData, prioritizeAccount: true);
```

### Processing Logic
Computation, transformation, formatting.
```csharp
// Compute: Total price with tax
order.Total = order.Subtotal * (1 + taxRate);

// Normalize: Phone numbers to E.164 format
var normalizedPhone = PhoneNumberUtil.FormatE164(customer.Phone, customer.Country);
```

### Flow Control & Coordination
Async operations and execution flow.
```csharp
// Retry: Database connection on transient failure
var result = await _retryPolicy.ExecuteAsync(() => _db.QueryAsync(sql));

// Fallback: Use cached data if service unavailable
var data = await _apiService.GetDataAsync() ?? _cache.GetCachedData();
```

### Resource Management
Connections, locks, cleanup.
```csharp
// Acquire: Distributed lock for inventory coordination
using var distributedLock = await _lockProvider.AcquireAsync(
    resource: $"inventory-{sku}", expiry: TimeSpan.FromMinutes(2));
```

### Events & Business Rules
```csharp
// Enforce: Minimum order amount business rule
if (order.Total < _settings.MinimumOrderAmount)
    throw new BusinessRuleViolationException(
        $"Order total ${order.Total} below minimum ${_settings.MinimumOrderAmount}");
```

### Integration & Communication
```csharp
// Webhook: Validate and process incoming payment notification
if (!_webhookValidator.IsValid(request.Headers, request.Body))
    return BadRequest("Invalid webhook signature");
```

### Error Handling & Recovery
```csharp
catch (PaymentException ex)
{
    // Compensate: Release reserved inventory
    await _inventory.ReleaseReservation(order.Items);

    // Degrade: Queue for manual processing instead of failing
    await _manualProcessingQueue.EnqueueAsync(order.Id);
}
```

### Observability & Debugging
```csharp
// Audit: Compliance logging for financial transactions
_auditLogger.LogFinancialTransaction(new AuditEvent
{
    UserId = order.CustomerId, Action = "OrderPayment",
    Amount = order.Total, Timestamp = _dateTime.UtcNow
});
```

## Anti-Patterns

**Redundancy** — the comment repeats the code.
```csharp
// Bad
// Assign: Set order ID to 123
order.Id = 123;

// Good
// Assign: Order ID from payment gateway response
order.Id = paymentResponse.OrderId;
```

**Vagueness** — the label doesn't say what actually happens.
```csharp
// Bad
// Process: Handle the order

// Good
// Validate: Order meets minimum purchase requirements
```

**Over-commenting** — every line gets a label.
```csharp
// Bad
// Create: New list
var items = new List<Item>();
// Add: First item
items.Add(item1);

// Good — one comment covers the intent behind both lines
// Create: Shopping cart seeded with customer's saved items
var items = new List<Item>();
items.AddRange(customer.SavedItems);
```

**Inconsistent labeling** — mixed casing and formats defeat grep.
```csharp
// Bad
// validate email format
// CHECK: user permissions

// Good
// Validate: Email format and domain rules
// Check: User permissions for resource access
```

**Stale comments** — the label no longer matches what the code does.
```csharp
// Bad — code grew, comment didn't
// Create: Simple product entity
var product = await ProductFactory.CreateWithInventoryTracking(
    sku, name, price, supplier, warehouseLocation);

// Good
// Create: Product with inventory management features
var product = await ProductFactory.CreateWithInventoryTracking(/*...*/);
```

## Best Practices

1. One action per label — no "and".
2. Capitalize the first word after the label.
3. Match the indentation of the code it describes.
4. Keep the line under ~80 characters.
5. Comment the *why*, not the *what* — business reasoning, not mechanics.
6. Use sparingly: reserve labels for non-obvious or critical operations.
7. Keep labeling consistent across the codebase — pick one vocabulary and stick to it.
8. Update the comment when the code it describes changes.

### Temporal markers
For comments with an expiry date or a known removal condition:
```csharp
// TODO-2026Q2: Replace with new payment provider API
// TEMP: Workaround for rate limiting until service upgrade
// DEADLINE-2026-06-15: Remove feature flag after full rollout
```

### Scaling detail to complexity
```csharp
// Simple operation — minimal comment
// Create: User session
var session = new UserSession(userId);

// Complex operation — the *why* needs more room
// Transform: Multi-currency totals using historical exchange rates,
// so financial reports stay consistent across regions over time
var reportingTotal = await _currencyService.ConvertWithHistoricalRates(
    order.Total, order.Currency, reportingCurrency, order.CreatedAt);
```

## Why this works

- **Scannable** — labels act as headlines for quick navigation.
- **Greppable** — search for `// Validate:` and find every validation point.
- **Durable** — a one-line label has less to go stale than a paragraph.
- **Language-agnostic** — the same vocabulary works in C#, Java, TypeScript, Python, and beyond.
