# TODO.md — Lista priorytetów

## Priorytety
- **P0** — Critical (blokujące ukończenie)
- **P1** — High (wymagane dla v1.0)
- **P2** — Medium (ważne ale opcjonalne)
- **P3** — Low (nice to have)

---

## P0 — Critical

### Systemy fundamentalne
- [ ] JSON Loader z obsługą błędów
- [ ] Item Database runtime
- [ ] NPC Database runtime
- [ ] Quest Manager z trackingiem
- [ ] Dialogue Manager z wyborami
- [ ] Save/Load System (JSON)

### Core gameplay
- [ ] Player Controller (ruch, kamera)
- [ ] Inventory System (add/remove/equip)
- [ ] Stats System (HP, MP, damage calc)
- [ ] Basic Combat (melee)
- [ ] Basic UI (HUD, menus)

### Zawartość minimalna
- [ ] 1 lokacja (vertical slice)
- [ ] 5 named NPCs
- [ ] 1 potwór
- [ ] 3 bronie (sword, bow, spell)
- [ ] 1 quest główny
- [ ] 3 dialogi

### Build
- [ ] Windows x64 build
- [ ] Brak crashy przy starcie
- [ ] Brak missing references

---

## P1 — High

### Systemy gameplay
- [ ] Lockpick minigame
- [ ] Stealth/Crime system
- [ ] Trainer system
- [ ] Skill system (4 skills)
- [ ] Day/Night cycle
- [ ] NPC schedules
- [ ] NPC AI (patrol, alert, combat)
- [ ] Enemy variety (6 types)

### Zawartość (v1.0)
- [ ] 8 lokacji (world map)
- [ ] 65 named NPCs
- [ ] 30 bronie (20 swords + 10 bows)
- [ ] 4 zbroje
- [ ] 20 roślin/potków
- [ ] 25 quests (5 main + 10 faction + 10 side)
- [ ] Pełne dialogi (all NPCs)
- [ ] 2 frakcje (Old Order + New Order)

### UI/UX
- [ ] Inventory panel
- [ ] Character sheet
- [ ] Quest journal
- [ ] Dialogue UI
- [ ] Main menu
- [ ] Pause menu
- [ ] Options menu (volume, graphics, controls)

### Polish
- [ ] VFX (hit effects, spells)
- [ ] Hit reactions (animations)
- [ ] Sound effects
- [ ] Music (ambient, combat)
- [ ] UI polish (transitions, feedback)

---

## P2 — Medium

### Zaawansowane systemy
- [ ] Crafting system
- [ ] Trading system (merchants)
- [ ] Reputation system
- [ ] NPC relationships
- [ ] Dynamic events
- [ ] Boss encounters

### Zawartość rozszerzona
- [ ] Hidden quests (easter eggs)
- [ ] Multiple solutions (stealth vs combat)
- [ ] Consequence system
- [ ] Multiple endings (2 endings + variations)
- [ ] New Game Plus

### Optymalizacja
- [ ] LOD system
- [ ] Object pooling
- [ ] Occlusion culling
- [ ] Performance profiling
- [ ] Memory optimization

### Narzędzia deweloperskie
- [ ] Editor tools (spawner, debugger)
- [ ] Quest tree visualizer
- [ ] Dialogue editor
- [ ] Data validator (standalone)

---

## P3 — Low

### Dodatki
- [ ] Photo mode
- [ ] Codex/lore viewer
- [ ] Achievements
- [ ] Statistics screen
- [ ] Modding support (data-driven)

### Accessibility
- [ ] Subtitles options
- [ ] Colorblind modes
- [ ] Keybinding remapping
- [ ] Difficulty settings

### Content expansion (v1.1+)
- [ ] Additional faction (neutral?)
- [ ] More regions
- [ ] DLC-ready architecture
- [ ] Multiplayer foundations?

---

## Blokery i zależności

### Nie można zakończyć bez:
1. ✅ JSON Loader → Item/NPC/Quest databases
2. ✅ Databases → Gameplay systems
3. ✅ Core gameplay → Content (quests, NPCs)
4. ✅ Content → Polish (VFX, audio)
5. ✅ All of above → Build and test

### Znane problemy
| Problem | Status | Rozwiązanie |
|---------|--------|-------------|
| Animation controller conflict | 🔴 AKTYWNE | Priorytetowe w P1 |
| NPC pathfinding edge cases | 🟡 OSTRZERZENIE | Fallback teleport |
| Save corruption edge case | 🟡 OSTRZERZENIE | Validation + backup |

---

## DONE — Zrealizowane

### Faza 0 (Setup)
- [x] Repository setup
- [x] Project structure
- [x] Assembly definitions
- [x] Namespace conventions
- [x] Documentation (12 files)

### Faza 0.5 (Data)
- [x] JSON schemas
- [x] Example data files
- [x] Data validator template
- [x] Save schema

---

## Metryki ukończenia

| Metric | Target | Current | % |
|--------|--------|---------|---|
| Lokacje | 8 | 1 | 12.5% |
| NPC | 65 | 5 | 7.7% |
| Potwory | 6 | 1 | 16.7% |
| Bronie | 30 | 3 | 10% |
| Questy | 25 | 1 | 4% |
| Dialogi | 50+ | 3 | 6% |
| Zbroje | 4 | 0 | 0% |
| UI screens | 10 | 3 | 30% |
