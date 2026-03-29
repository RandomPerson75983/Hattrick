# Phase 8: Transfer Market (Simplified)

**Goal:** Buy from pool at fixed price. Sell at fixed price. No AI bidding.

**Deliverable:** Transfer market works. Player valuation sensible. AI manages squads.

---

## Transfer Windows

- Summer: week 15-16 + weeks 1-2 of new season
- Winter: week 8 (1-week window)

## Services (all methods take `Guid teamId`)

- `IPlayerValuationService` - market value = f(TSI, age); deduct 8% commissions
- `ITransferMarketService(teamId, ...)` - pool generation, buy/sell
- AI transfer logic handled by `IRulesBasedAi` + `IAiManagerOrchestrator` (Phase 4)
  - Adds transfer tool definitions to `ToolDefinitionProvider`
  - Adds transfer methods to `IRulesBasedAi`: buy weakness, sell old players
