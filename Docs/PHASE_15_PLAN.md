# Phase 15: Expanded Transfer Market

**Goal:** Replace simplified buy/sell with real transfers across leagues and countries. AI teams actively buy and sell players.

**Deliverable:** Players can be transferred between teams across divisions and countries. AI managers make realistic transfer decisions. Market prices reflect supply and demand.

---

## Features

- Cross-division transfers (buy from Division 2, sell to Division 1)
- Cross-country transfers (international transfers)
- Transfer listing with bidding period
- AI teams actively participate in transfer market
- Market value influenced by:
  - Player skills, age, potential
  - Division level of selling/buying team
  - Supply and demand dynamics
- Transfer history tracking
- Foreign player wage premium (+20% for non-native players)

## Services

- Extend `ITransferMarketService` - multi-league pool, bidding, cross-country transfers
- `ITransferBiddingService` - manage bid periods and resolution
- Extend `IRulesBasedAi` - smarter transfer logic across divisions (buy from lower divisions, sell aging players)
- Extend `IPlayerValuationService` - dynamic market pricing based on division context

## Transfer Window Rules

- Summer: week 15-16 + weeks 1-2 of new season (same as Phase 8)
- Winter: week 8 (1-week window)
- International transfers: available in both windows
- Commission: 8% of transfer fee
