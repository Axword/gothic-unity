# CHALLENGES.md

## Audyt 2026-07-19

- **Brak silnika w środowisku:** nie wolno zastępować testu Unity twierdzeniem, że kod działa. Statusy builda i Play Mode są BLOCKED.
- **Dwa poziomy runtime:** demonstracyjny `VerticalSlice*` nie jest jeszcze produkcyjnym `QuestManager`/`DialogueManager`. Wymagana jest integracja, a nie dalsze zwiększanie atrap.
- **Dane vs dokumentacja:** audyt wykazał faktyczne liczby niższe od celów. Dokumentacja została uzupełniona o jawny stan zamiast ukrywania braków.
 — Wyzwania techniczne i projektowe

## 1. Wyzwania główne

### 1.1 Modularność vs. prostota

**Problem:** Wymagana modularna architektura (5 assembly definitions) może wprowadzać złożoność dla małego zespołu.

**Decyzja:** Mimo alles assembly, trzymać zależności proste. Unikać premature abstraction. Core → Data → Gameplay to jedyne zależności w dół.

**Kompromis:** Dla v0.9 można zacząć z jednym assembly i refactorować później. Ale structura katalogów już teraz.

---

### 1.2 JSON vs. ScriptableObjects

**Problem:** Wymaganie: "ScriptableObjects wyłącznie jako importowane reprezentacje danych, jeśli nie łamie to wymogu JSON"

**Interpretacja:** 
- Canonical data = JSON
- Runtime representations = ScriptableObjects generated from JSON
- Oba są dozwolone, ale JSON jest master

**Decyzja:** 
- Load JSON at startup → Generate ScriptableObjects in memory
- OR: JSON Schema validator runs in editor, runtime uses generated ScriptableObjects
- Dla v0.9: Direct JSON parsing, no ScriptableObjects

---

### 1.3 NPC AI State Management

**Problem:** NPC musi obsługiwać wiele stanów jednocześnie (schedule-based location + event-based reactions + combat).

**Decyzja:** Hierarchical State Machine
- Top level: Schedule (where am I supposed to be?)
- Second level: Activity (what am I doing?)
- Third level: Events (reaction to player, combat)

```csharp
// Koncept
ScheduleState
├── AtHome → Idle → Work → Social → Eat → Sleep
├── Traveling → WalkToMarker → WalkToMarker → ...
├── Alerted → Investigate → Chase → Combat → Return
└── Dead → (cleanup)
```

---

### 1.4 Combat Design

**Problem:** Balans między czytelnością (stamina system?) a prostotą (bez stamina?).

**Decyzja:** Prosty system bez stamina dla v1.0
- Attack speed = weapon property
- Block = timing-based (harder for player, more satisfying)
- No stamina = more accessible, less frustrating
- Optional stamina in v1.1 if time permits

**Ale:** Hit detection musi być bardzo czytelny (VFX, sounds, screen shake).

---

### 1.5 Quest Dependency Graph

**Problem:** Zadania mają skomplikowane zależności. Bug w jednym może zablokować wiele.

**Decyzja:** 
- Quest Manager z pełną walidacją grafu przy ładowaniu
- Każdy quest ma jasne requirements
- Debug mode pokazuje status wszystkich questów
- Quest journal pokazuje drzewo

---

### 1.6 Crime System

**Problem:** Reakcja NPC na przestępstwa wymaga świadków, linii wzroku, pamięci.

**Decyzja:** Simplified witness system
- NPC ma listę "widziałem" flag
- "Widziałem" resetuje się po czasie lub poza zasięgiem
- Alerty propagują się do NPC w zasięgu
- Kradzież = flag na graczu + flag na NPC
- Brak wszechwiedzącego systemu

---

### 1.7 Save Game Integrity

**Problem:** JSON save może się corrupted. Gracze mogą manualnie edytować.

**Decyzja:**
- Checksum (hash) dla całego save'a
- Version schema dla migracji
- Backup previous save on overwrite
- Validation on load (repair or fallback to default)

---

## 2. Kompromisy (Trade-offs)

### 2.1 Graphics vs. Performance

| Kompromis | Decyzja |
|-----------|---------|
| LOD dla wszystkiego | Tak, nawet małych obiektów |
| Real-time shadows | Tylko directional, no point light shadows |
| Post-processing | Prosty (vignette, color grade) |
| Draw calls | Instancing dla roślinności |

### 2.2 Content vs. Polish

| Kompromis | Decyzja |
|-----------|---------|
| Więcej NPC vs. lepsze NPC | Jakość > ilość. 65 named > 200 anonymous |
| Więcej broni vs. lepsze modele | 20 broni z modelami > 100 z cubes |
| Więcej questów vs. lepsze dialogi | 25 z pełnymi dialogami > 100 z one-liner |

### 2.3 Animation vs. Scope

| Kompromis | Decyzja |
|-----------|---------|
| Full combat combos | Tak, dla immersion |
| Procedural animations | Nie, zbyt ryzykowne |
| IK hands | Podstawowe, tylko dla 2H weapons |
| Facial animations | Nie, body language only |

---

## 3. Ryzyka (Risk Assessment)

### Wysokie ryzyko

| Ryzyko | Prawdopodobieństwo | Wpływ | Mitigation |
|--------|-------------------|-------|------------|
| Animation conflicts | Wysokie | Wysoki | Early prototyping, clear state machine |
| Save corruption | Średnie | Wysoki | Backup system, validation |
| Quest bugs blocking progression | Średnie | Wysoki | Quest debugger, multiple solutions |

### Średnie ryzyko

| Ryzyko | Prawdopodobieństwo | Wpływ | Mitigation |
|--------|-------------------|-------|------------|
| Performance issues | Średnie | Średni | Early profiling, LOD system |
| NPC pathfinding fails | Wysokie | Niski | Fallback teleport, simple navmesh |
| Missing content at deadline | Wysokie | Średni | Scope cut, ship v0.9 |

### Niskie ryzyko

| Ryzyko | Prawdopodobieństwo | Wpływ | Mitigation |
|--------|-------------------|-------|------------|
| JSON schema changes | Niskie | Średni | Versioning, migration scripts |
| UI doesn't match design | Niskie | Niski | Prototyping early |
| Sound assets missing | Niskie | Niski | Placeholder sounds, procedural fallback |

---

## 4. Architektura decyzje (ADR)

### ADR-001: Assembly Definition Structure
```
Core (no dependencies)
Data (depends on Core)
Gameplay (depends on Core, Data)
AI (depends on Core, Data)
UI (depends on Core, Data, Gameplay)
```

### ADR-002: Data Flow
```
JSON → JsonLoader → Runtime Objects → ScriptableObjects
                     ↓
              Game Systems
```

### ADR-003: State Machine Pattern
```
State Machine Base
├── PlayerStateMachine
├── NpcStateMachine
└── CombatStateMachine
```

### ADR-004: No Stamina System
- Simplified combat
- Weapon speed = attack rate
- Block = timing window

### ADR-005: Schedule-based NPC
```
TimeManager → triggers → NPC brain → state change
```

### ADR-006: Quest Flags over State
```
Quest State = flags + objectives completed
NOT: complex enum tracking
```

---

## 5. Lessons Learned (na bieżąco)

### v0.1: Start z dokumentacji
Dokumentacja PRZED kodem oszczędza refaktoryzację.

### v0.2: Greyboxing first
Nie pisać finalnych assetów, dopóki gameplay nie działa.

### v0.3: Data-driven everything
Wszystko z JSON, nawet jeśli "prościej" byłoby hardcoded.

### v0.4: Test early
Combat prototype w tygodniu 1, nie tydzień 8.

---

## 6. Znane issues

| Issue | Severity | Workaround |
|-------|----------|------------|
| NPC path through walls | Medium | Simple navmesh, agent radius |
| Quest tracker UI delay | Low | Acceptable, update on change |
| Save file size large | Low | Compression if needed |
| Memory usage high | Medium | Object pooling, LOD |
