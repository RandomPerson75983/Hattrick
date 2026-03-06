# Development Rules & Standards

This is a reference document with examples. See CLAUDE.md for the authoritative rules.

## Core Principles

- **Don't be lazy.** Don't defer any issues you find.
- **TDD always.** Write failing test FIRST, then implementation.
- **Reproduce bugs before fixing.** Never guess; trace the code path.
- **Run full test suite** after every change: `dotnet test Hattrick.Tests/Hattrick.Tests.csproj`

---

## Architecture Rules

### Blazor Components Have ZERO Business Logic

❌ **Bad:**
```csharp
// TeamsPage.razor.cs
public class TeamsPage : ComponentBase
{
    private void UpdateTeam()
    {
        team.Budget -= player.Wage;  // BUSINESS LOGIC IN COMPONENT!
    }
}
```

✅ **Good:**
```csharp
// TeamsPage.razor.cs
public partial class TeamsPage : ComponentBase
{
    [Inject] ITeamService TeamService { get; set; }

    private async Task UpdateTeam()
    {
        await TeamService.UpdateTeam(team);
    }
}
```

Components call services. Services contain all logic.

### All Services Have Interfaces

❌ **Bad:**
```csharp
new TrainingService().TrainPlayer(playerId);
```

✅ **Good:**
```csharp
[Inject] ITrainingService TrainingService { get; set; }
await TrainingService.TrainPlayer(playerId);
```

Every service: Interface → Implementation. Registered in DI.

### Constructor Injection Only

❌ **Bad:**
```csharp
public class SeasonService
{
    private IPlayerRepository _playerRepo = new PlayerRepository();
}
```

✅ **Good:**
```csharp
public class SeasonService
{
    private readonly IPlayerRepository _playerRepo;

    public SeasonService(IPlayerRepository playerRepo)
    {
        _playerRepo = playerRepo;
    }
}
```

Never `new Service()` inside another class.

### Function Size Limits

- **Functions:** ≤ 30 lines (one job = one method)
- **Classes:** ≤ 300 lines (consider decomposition)

If you exceed these, split the logic.

### Gameplay vs Persistence

- **In-memory:** All gameplay logic, calculations, simulations
- **JSON files:** Save/load persistence only

No database. No Entity Framework. Pure in-memory objects + JSON serialization.

---

## Code Quality: Top 10 Rules

### 1. No Magic Numbers

❌ **Bad:**
```csharp
player.Skill += 0.5;  // where does 0.5 come from?
if (age > 28) { }  // what's the significance of 28?
```

✅ **Good:**
```csharp
const decimal SkillGainPerMinute = 0.5m / 90m;
player.Skill += SkillGainPerMinute;

const int PeakAgeYear = 28;
if (age > PeakAgeYear) { }
```

Every numeric literal → named constant with clear name + comment if non-obvious.

### 2. CultureInfo.InvariantCulture

❌ **Bad:**
```csharp
string formatted = player.Skill.ToString("0.00");  // locale-dependent!
```

✅ **Good:**
```csharp
string formatted = player.Skill.ToString("0.00", CultureInfo.InvariantCulture);
```

Always use InvariantCulture for `.ToString("format")` calls.

### 3. Null-Check Before .Value

❌ **Bad:**
```csharp
MatchResult result = match.Result!;  // relying on ! is brittle
int goals = result.HomeGoals;
```

✅ **Good:**
```csharp
if (match.Result == null)
{
    throw new InvalidOperationException("Match not yet played");
}
MatchResult result = match.Result;
int goals = result.HomeGoals;
```

Or use guard clauses:
```csharp
var result = match.Result ?? throw new InvalidOperationException("Match not yet played");
```

### 4. Async Void Try-Catch

❌ **Bad:**
```csharp
// LineupPage.razor
private async void UpdateLineup()  // async void without try-catch!
{
    await LineupService.UpdateLineup(lineup);
}
```

✅ **Good:**
```csharp
private async Task UpdateLineup()  // return Task, not void
{
    try
    {
        await LineupService.UpdateLineup(lineup);
        await InvokeAsync(StateHasChanged);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to update lineup");
        // show error to user
    }
}
```

Or use async void event handlers with try-catch:
```csharp
private async void OnButtonClick()
{
    try
    {
        await DoSomethingAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error in button click");
    }
}
```

### 5. IRandomProvider for All Randomness

❌ **Bad:**
```csharp
int goalChance = Random.Shared.Next(1, 100);
int homeGoals = new Random().Next(10);  // creates new instance!
```

✅ **Good:**
```csharp
public class MatchSimulator
{
    private readonly IRandomProvider _random;

    public MatchSimulator(IRandomProvider random)
    {
        _random = random;
    }

    public int SimulateGoals()
    {
        return _random.Next(1, 100);
    }
}
```

All randomness flows through IRandomProvider (injected). Enables reproducible tests.

### 6. IDateTimeProvider for All Time

❌ **Bad:**
```csharp
public class TrainingService
{
    public void UpdateForm()
    {
        DateTime now = DateTime.Now;  // service can't be tested!
        // ...
    }
}
```

✅ **Good:**
```csharp
public class TrainingService
{
    private readonly IDateTimeProvider _dateTime;

    public TrainingService(IDateTimeProvider dateTime)
    {
        _dateTime = dateTime;
    }

    public void UpdateForm()
    {
        DateTime now = _dateTime.Now;
        // ...
    }
}
```

Never `DateTime.Now` in services. Use IDateTimeProvider.

### 7. IReadOnlyList Returns from Repositories

❌ **Bad:**
```csharp
public List<Player> GetAllPlayers()
{
    return _players;  // caller can modify!
}
```

✅ **Good:**
```csharp
public IReadOnlyList<Player> GetAllPlayers()
{
    return _players.AsReadOnly();
}
```

Repositories return IReadOnlyList, never List<T> directly.

### 8. Thread-Safe Locks on Repositories

❌ **Bad:**
```csharp
public class PlayerRepository : IPlayerRepository
{
    private List<Player> _players = new();

    public void Add(Player p)
    {
        _players.Add(p);  // not thread-safe!
    }
}
```

✅ **Good:**
```csharp
public class PlayerRepository : IPlayerRepository
{
    private readonly List<Player> _players = new();
    private readonly Lock _lock = new();

    public void Add(Player player)
    {
        lock (_lock)
        {
            _players.Add(player);
        }
    }

    public IReadOnlyList<Player> GetAll()
    {
        lock (_lock)
        {
            return _players.AsReadOnly();
        }
    }
}
```

All repositories: `private readonly Lock _lock = new();` + lock all read/write operations.

### 9. Test Naming Convention

❌ **Bad:**
```csharp
[Fact]
public void TestSkill() { }

[Fact]
public void UpdateTest() { }
```

✅ **Good:**
```csharp
[Fact]
public void UpdatePlayerSkill_WhenTrainingForOneWeek_SkillIncreasesByExpectedAmount()
{
    // Arrange
    var player = new Player { Skill = 5 };

    // Act
    player.TrainForWeek(intensity: 100);

    // Assert
    player.Skill.Should().BeGreaterThan(5);
}
```

Format: `MethodName_Scenario_ExpectedResult()`

### 10. bUnit v2 for Blazor Tests

❌ **Bad:**
```csharp
// Old bUnit v1 API
var component = RenderComponent<LineupPage>();
```

✅ **Good:**
```csharp
// bUnit v2 API
[Fact]
public void LineupPage_WhenLoaded_DisplaysPlayerPositions()
{
    using var ctx = new TestContext();
    var cut = ctx.Render(@<LineupPage />);

    cut.WaitForAssertion(() =>
        cut.FindAll("div.player-position").Should().NotBeEmpty()
    );
}
```

Use `TestContext` + `Render<T>()` for bUnit v2.

---

## Common Patterns

### Service Validation

Always validate at system boundaries (user input, external APIs). Don't over-validate internal operations.

```csharp
public class LineupService
{
    public void SetStartingLineup(Guid teamId, List<MatchLineupSlot> slots)
    {
        // Validate input (system boundary)
        if (slots == null || slots.Count != 11)
            throw new ArgumentException("Must have exactly 11 starters");

        // Trust internal invariants
        var team = _teamRepo.GetById(teamId);
        // (assume team exists — already validated on load)

        team.DefaultLineup = new TeamLineup { Slots = slots };
        _teamRepo.Update(team);
    }
}
```

### Save Pattern (Atomic Temp-Rename)

```csharp
public class SaveGameService
{
    public void SaveGame(Game game, int slotNumber)
    {
        string slotPath = $"saves/slot_{slotNumber}.json";
        string tempPath = $"{slotPath}.tmp";

        try
        {
            // Write to temp file
            var json = JsonSerializer.Serialize(game, _options);
            File.WriteAllText(tempPath, json);

            // Atomic rename
            File.Delete(slotPath);  // ok if doesn't exist
            File.Move(tempPath, slotPath);
        }
        catch
        {
            File.Delete(tempPath);  // cleanup on failure
            throw;
        }
    }
}
```

Prevents corruption if write fails mid-way.

### Error Handling in Match Simulator

Don't catch everything. Be specific.

```csharp
public MatchResult SimulateMatch(Team home, Team away)
{
    if (home?.Players == null)
        throw new ArgumentNullException(nameof(home), "Home team must have players");

    try
    {
        return _simulatorEngine.Run(home, away);
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Invalid lineup"))
    {
        throw new InvalidOperationException(
            "Cannot simulate match: invalid team lineup configuration", ex);
    }
    catch (Exception ex)
    {
        // Log unexpected errors
        _logger.LogError(ex, "Unexpected error during match simulation");
        throw;
    }
}
```

---

## Common Mistakes to Avoid

### ❌ Deferred Execution in Loops

```csharp
// BAD: closure captures loop variable
var queries = new List<IQueryable<Player>>();
foreach (var skill in skills)
{
    queries.Add(_playerRepo.GetAll().Where(p => p.SkillLevel == skill));
    // ^^^ this won't work as expected due to deferred execution
}
```

✅ **Good:**
```csharp
var queries = skills.Select(skill =>
    _playerRepo.GetAll().Where(p => p.SkillLevel == skill)
).ToList();
```

Or materialize inside the loop:
```csharp
foreach (var skill in skills)
{
    var filtered = _playerRepo.GetAll()
        .Where(p => p.SkillLevel == skill)
        .ToList();  // materialize here
}
```

### ❌ Forgetting to Await

```csharp
// BAD
public void SimulateWeek()
{
    _trainingService.TrainAllPlayers();  // forgot await!
}

// GOOD
public async Task SimulateWeek()
{
    await _trainingService.TrainAllPlayersAsync();
}
```

### ❌ Mixing Sync and Async

```csharp
// BAD: never do this
public async Task<int> GetPlayerCount()
{
    return await GetCountAsync().ConfigureAwait(false).GetAwaiter().GetResult();
}

// GOOD: all the way async
public async Task<int> GetPlayerCountAsync()
{
    return await _repo.CountAsync();
}
```

### ❌ Over-Engineering for Hypothetical Scenarios

```csharp
// BAD: "we might need to support 3 match types someday"
public interface IMatchTypeFactory
{
    IMatch CreateMatch(MatchTypeEnum type);
}

// GOOD: support exactly what you need now
public class MatchSimulator
{
    public LeagueMatch SimulateLeagueMatch(Team home, Team away) { }
    public CupMatch SimulateCupMatch(Team home, Team away) { }
}
```

---

## Git Workflow

### Before Committing

```bash
# Run tests
dotnet test Hattrick.Tests/Hattrick.Tests.csproj

# Check formatting (if using stylecop)
dotnet format Hattrick/ --verify-no-changes

# Check for unintended changes
git diff
git status
```

### Commit Messages

```bash
# Feature
git commit -m "feat: Add player training system"

# Bug fix
git commit -m "fix: Correct goal probability calculation in match simulator"

# Test
git commit -m "test: Add regression test for lineup validation"

# Refactor
git commit -m "refactor: Extract RatingCalculation into separate service"
```

---

## Debugging Tips

### IRandomProvider Testing

```csharp
[Fact]
public void MatchEngine_WithSeededRandom_ProducesReproducibleResults()
{
    var random = new SeededRandomProvider(seed: 12345);
    var match1 = _simulator.Simulate(home, away, random);

    random = new SeededRandomProvider(seed: 12345);
    var match2 = _simulator.Simulate(home, away, random);

    match1.HomeGoals.Should().Be(match2.HomeGoals);
}
```

### IDateTimeProvider Testing

```csharp
[Fact]
public void TrainingService_UpdatesFormAtExpectedTime()
{
    var fakeTime = new FakeDateTimeProvider(new DateTime(2026, 3, 7));
    var service = new TrainingService(fakeTime);

    // Act
    service.UpdateForm(player);

    // Assert: form changed based on exact date
}
```

---

See CLAUDE.md for the authoritative rules. This document is a reference guide with examples.
