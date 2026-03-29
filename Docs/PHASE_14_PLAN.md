# Phase 14: Country Creation

**Goal:** Support multiple countries, each with their own league pyramid. Different countries have different league sizes.

**Deliverable:** Player can choose a country when starting a new game. Each country has its own league structure with appropriate team/player names.

---

## Features

- Country selection at new game start
- Each country has its own league pyramid (1-3 divisions)
- Country-specific fictional team and player name pools
- Different countries may have different division sizes (6, 8, or 10 teams per division)
- National team representation (players from each country)

## Services

- `ICountryService` - manage available countries and their configurations
- Extend `INameGenerationService` - country-specific name pools for teams and players
- Extend `ILeagueStructureService` - country-specific division sizes and rules
- Extend `IDivisionSetupService` - generate divisions per country configuration

## Reference

- `wiki_archive/Countries.html` - list of Hattrick countries
- Country-specific wiki pages in `wiki_archive/` for naming conventions
