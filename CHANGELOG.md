# CHANGELOG.md — Historia zmian

Wszystkie istotne zmiany w projekcie. Format: `YYYY-MM-DD - [TYP] Opis`

---

## [0.5.0] - 2024-07-19 - Fundamenty projektu

### Dodane
- ✅ Pełna architektura projektu z 5 Assembly Definitions
  - ZelaznaDroga.Core (podstawowe narzędzia)
  - ZelaznaDroga.Data (JSON loader, schematy)
  - ZelaznaDroga.Gameplay (logika gry)
  - ZelaznaDroga.AI (nawigacja, zachowania)
  - ZelaznaDroga.UI (interfejs)
- ✅ System przestrzeni nazw (namespace) - ZelaznaDroga.*
- ✅ Core utilities:
  - BaseMonoBehaviour (bazowa klasa)
  - ComponentLocator (service locator)
  - EventBus (pub/sub messaging)
  - Scheduler (opóźnione zadania)
  - UniqueIdGenerator
  - GameConstants
- ✅ Extensions (Unity + Collections)
- ✅ Data layer:
  - JsonDataLoader (ładowanie JSON)
  - SaveDataManager (zapis/wczyt)
  - Pełny schema system (GameDataSchemas.cs)
  - Wszystkie typy danych: Item, NPC, Monster, Quest, Dialogue, Spell, Trainer, Location, Loot, Save
- ✅ Gameplay systems:
  - PlayerController (ruch, kamera, input)
  - PlayerStats (statystyki, leveling, HP/MP)
  - InventorySystem (ekwipunek, equip, gold)
  - CombatSystem (ataki, blok, casty)
  - QuestManager (zarządzanie questami)
  - DialogueManager (dialogi, wybory, warunki)
  - TimeManager (dzień/noc, czas gry)
- ✅ Event system (pełna lista eventów gry)
- ✅ Interfaces dla wszystkich systemów (IOC)

### Dane JSON
- ✅ items_weapons_swords.json (8 mieczy)
- ✅ items_weapons_bows.json (5 łuków)
- ✅ items_armors.json (6 zbroi)
- ✅ items_plants.json (10 roślin)
- ✅ items_potions.json (6 mikstur)
- ✅ items_misc.json (14 różnych przedmiotów)
- ✅ npcs.json (9 NPC - Aldona, Boruk, Młynarczyk, Zosia, Drwal, Grom, Płomienny, Kosa, Wędrowiec)
- ✅ npc_schedules.json (5 harmonogramów NPC)
- ✅ monsters.json (6 stworów - wilk, niedźwiedź, bandyta, trol, błotna bestia, pająk)
- ✅ quests_main.json (4 główne questy)
- ✅ quests_old_faction.json (6 questów Gildii)
- ✅ quests_new_faction.json (6 questów Wolnych)
- ✅ quests_side.json (3 questy poboczne)
- ✅ dialogues_main.json (Wędrowiec intro)
- ✅ dialogues_aldona.json (Mistrzyni Gildii)

### Dokumentacja
- ✅ README.md
- ✅ GAME_DESIGN.md
- ✅ WORLD_AND_LORE.md
- ✅ QUESTS_AND_DIALOGUES.md
- ✅ ARCHITECTURE.md
- ✅ DATA_SCHEMAS.md
- ✅ ART_BIBLE.md
- ✅ CHANGELOG.md
- ✅ PROGRESS.md
- ✅ TODO.md
- ✅ CHALLENGES.md
- ✅ TEST_PLAN.md
- ✅ THIRD_PARTY_ASSETS.md

---

## [0.1.0] - 2024-01-15 - Init

### Dodane
- Inicjalizacja repozytorium
- Szablon README.md
- Plan projektu w GAME_DESIGN.md
