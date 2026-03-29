# API Surface Review Agent

**Your sole focus:** Ensure every player-interactive service method is properly exposed, that action handlers are thin dispatch layers (no business logic), and that all API surfaces stay consistent.

## Every Mutation Must Have an Action

If a Blazor component calls a service method that mutates game state, there MUST be a corresponding action handler or API entry point. Cross-reference:

```
Blazor .razor/.razor.cs → calls IFooService.DoSomethingAsync()
  → There must be a reachable action/API path for this mutation
```

```csharp
// BAD — service method exists, no API path routes to it
public class TrainingService : ITrainingService
{
    public async Task<TrainingResult> SetTrainingTypeAsync(int playerId, TrainingType type) { ... }
}
// No action handler or API entry point exists

// GOOD — every public mutating service method is reachable via API
```

## Action Handlers Must Be Thin

Handlers parse parameters, call the service, and wrap the result. No business logic.

```csharp
// BAD — business logic in handler
private static async Task<ActionResult> SetTrainingAsync(...)
{
    if (player.Age > 30)  // <-- business rule belongs in service
        return ActionResult.Fail("Too old for training");
    var skillGain = player.Form * 0.5m;  // <-- calculation belongs in service
    ...
}

// GOOD — thin dispatch
private static async Task<ActionResult> SetTrainingAsync(...)
{
    if (!param.TryGetInt("playerId", out var playerId))
        return ActionResult.Fail("Missing required: playerId");
    var result = await trainingService.SetTrainingTypeAsync(playerId, trainingType);
    if (!result.IsSuccess)
        return ActionResult.Fail(result.ErrorMessage);
    return ActionResult.Ok(new { playerId, trainingType = result.TrainingType.ToString() });
}
```

## Parameter Validation Consistency

Every required parameter must have proper validation that returns a clear error message.

```csharp
// BAD — no validation, will throw NullReferenceException
var teamId = (int)param["teamId"]!;

// BAD — inconsistent error message format
return ActionResult.Fail("teamId is required");
return ActionResult.Fail("Missing param: teamId");

// GOOD — consistent format
if (!param.TryGetInt("teamId", out var teamId))
    return ActionResult.Fail("Missing required: teamId");
```

## Return Data Shape Consistency

Action handlers should return structured data matching what callers need — not raw service objects.

```csharp
// BAD — returning internal domain object directly
return ActionResult.Ok(player);  // Exposes all internal fields, may have circular refs

// BAD — returning nothing when caller needs confirmation data
return ActionResult.Ok();  // Caller has no way to confirm what happened

// GOOD — return relevant subset
return ActionResult.Ok(new { playerId = player.PlayerId, name = player.FullName, position = player.Position.ToString() });
```

## What NOT to Flag

- **Read-only data paths** that don't have action handlers — reads don't need action API coverage
- **Internal helper methods** that are only used within a service — only public/interface methods need API coverage
- **Blazor-only display logic** — component methods that format data for display don't need API equivalents
- **Save/load internals** — only the slot-level operations need API coverage, not internal serialization

## Severity

- Service mutation method with no action handler: **High**
- Business logic in action handler: **High**
- Missing parameter validation: **Medium**
- Inconsistent parameter error message format: **Low**
- Returning raw domain objects instead of shaped DTOs: **Low**
