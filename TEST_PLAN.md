# TEST_PLAN.md — Plan testów

## 1. Typy testów

### 1.1 Testy automatyczne (Unit/Integration)
- JsonDataLoader validation
- Damage calculations
- XP/Level formulas
- Quest state transitions
- Save/Load serialization

### 1.2 Testy funkcjonalne (Play Mode)
- Smoke tests (każdy feature działa)
- Quest progression tests
- Combat tests (all weapons)
- Dialog tests (all branches)
- Save/Load tests

### 1.3 Testy regresyjne
- Po każdej zmianie
- Critical paths

### 1.4 Playtests
- Kanał alfa (zespół)
- Kanał beta (zewnętrzni)

---

## 2. Scenariusze testowe

### 2.1 Smoke Test — Nowa gra

```
PRE: Czysta instalacja, brak save
STEP: Kliknij "Nowa Gra"
EXPECT: Ładowanie → Scena świata → Kontrola gracza
STEP: Porusz się WASD
EXPECT: Gracz się porusza
STEP: Rozejrzyj się myszą
EXPECT: Kamera się obraca
STEP: Podejdź do NPC
EXPECT: Pojawia się prompt "E — Rozmowa"
STEP: Naciśnij E
EXPECT: Dialog się otwiera
PASS: Smoke test passed
```

### 2.2 Smoke Test — Combat

```
PRE: Miecz w inventory
STEP: Naciśnij E (equip)
EXPECT: Miecz w ręku
STEP: Podejdź do wilka
EXPECT: Wilk reaguje
STEP: Naciśnij LPM
EXPECT: Atak + animacja
STEP: Wilk atakuje
EXPECT: Gracz traci HP
STEP: Naciśnij PPM
EXPECT: Blok + animacja
PASS: Combat smoke test
```

### 2.3 Quest Progression Test

```
PRE: Q_M_001 not started
STEP: Complete objectives 1-3
EXPECT: Quest stage advances
STEP: Complete all stages
EXPECT: Quest marked complete
EXPECT: XP awarded
EXPECT: Q_M_002 unlocks
PASS: Quest progression test
```

### 2.4 Dialog Branch Test

```
PRE: Dialog with 3 choices
STEP: Choice 1
EXPECT: Node 1 + action executed
STEP: Reload save
STEP: Choice 2
EXPECT: Node 2 + different action
STEP: Reload save
STEP: Choice 3 (requires level 5)
EXPECT: [Locked] if level < 5
EXPECT: Choice works if level >= 5
PASS: Dialog branches test
```

### 2.5 Save/Load Test

```
PRE: Mid-game state (quest active, items, stats)
STEP: Open menu → Save
EXPECT: Save slot filled
STEP: Load game
EXPECT: Exact same state
STEP: Delete save
STEP: Load
EXPECT: Error message OR new game
PASS: Save/Load test
```

### 2.6 Lockpick Minigame Test

```
PRE: Chest with level 2 lock, 5 lockpicks
STEP: Open chest
EXPECT: Minigame starts
STEP: Fail 2 times
EXPECT: 3 lockpicks remaining
STEP: Succeed on 3rd
EXPECT: Chest opens
STEP: Loot taken
EXPECT: Loot removed from world
PASS: Lockpick test
```

### 2.7 Crime System Test

```
PRE: Steal from NPC with no witnesses
STEP: Take item from NPC inventory
EXPECT: Item in player inventory
EXPECT: NPC relation -5
STEP: No other NPC reacts
PASS: Stealth theft test

PRE: Steal from NPC with witness
STEP: Take item while witness watches
EXPECT: Witness alerts nearby NPCs
EXPECT: Player flagged as criminal
EXPECT: Combat initiates
PASS: Witnessed theft test
```

### 2.8 Trainer Test

```
PRE: Player level 3, 2 learning points
STEP: Talk to trainer
EXPECT: Training options available
STEP: Select "Sword Training"
EXPECT: Costs 1 point + gold
STEP: Confirm
EXPECT: Skill increased
EXPECT: Points decreased
STEP: Try to train without points
EXPECT: "Not enough points" message
PASS: Trainer test
```

### 2.9 Day/Night Cycle Test

```
PRE: Time 12:00 (noon)
STEP: Wait 10 minutes real time
EXPECT: Time 14:00
EXPECT: Lighting changes
STEP: Wait until 22:00
EXPECT: NPCs go to sleep
STEP: Wait until 6:00
EXPECT: NPCs wake up
PASS: Day/night test
```

### 2.10 Full Playthrough Test

```
PRE: None
STEP: New Game
STEP: Complete Q_M_001
STEP: Go to both camps
STEP: Do 3 Old Order quests
STEP: Choose Old Order
EXPECT: New Order locked
STEP: Complete Old Order final
STEP: Watch epilog
STEP: Save game
STEP: Reload
EXPECT: Epilog state preserved
PASS: Full playthrough test
```

---

## 3. Checklisty weryfikacji

### 3.1 Build Checklist

```
□ Project compiles without errors
□ All scenes build successfully
□ No missing references in build
□ All prefabs have thumbnails
□ All materials have textures
□ All animations are imported
□ Sound files are imported
□ No console errors on startup
□ Performance: 30+ FPS minimum
□ Memory: Under 2GB RAM usage
```

### 3.2 Content Checklist

```
□ 20 swords with models
□ 10 bows with models
□ 4 armors (player accessible)
□ 10 plants
□ 6 potions
□ ~6 monster types
□ 65 named NPCs
□ 25 quests (all completable)
□ All NPCs have schedules
□ All NPCs have at least greeting
□ Faction leaders have full dialogues
□ All items have icons
□ All items have descriptions
```

### 3.3 Feature Checklist

```
□ WASD movement
□ Mouse camera
□ Sprint
□ Jump
□ Melee combat (sword)
□ Ranged combat (bow)
□ Magic combat
□ Block/Dodge
□ Inventory (open/add/remove/equip)
□ Character sheet
□ Quest journal
□ Dialogue system
□ Lockpick minigame
□ Stealing
□ Skinning
□ Trainers
□ Day/night cycle
□ NPC schedules
□ NPC AI (patrol/alert/combat)
□ Save/Load
□ Main menu
□ Pause menu
□ Options
```

---

## 4. Test cases — Edge cases

### 4.1 Quest Edge Cases

```
TC-001: Quest giver dies before giving quest
  → Quest auto-removed or given by another NPC
  
TC-002: Quest target dies outside quest
  → Objective not auto-complete, must verify
  
TC-003: Player has max items, tries to collect
  → "Inventory full" message, item stays
  
TC-004: Quest requires item player sold
  → Item gone (consequence), quest may fail
  
TC-005: Player attacks quest NPC
  → Depends on NPC (essential vs non)
```

### 4.2 Combat Edge Cases

```
TC-010: Player attacks during dialog
  → Dialog closes, combat starts
  
TC-011: Enemy dies mid-animation
  → Animation completes, death triggers
  
TC-012: Both player and enemy die simultaneously
  → Player death takes priority, respawn
  
TC-013: Player spam-clicks attack
  → Attack speed enforced by weapon
  
TC-014: Arrow hits wall
  → Arrow destroyed, no damage
```

### 4.3 Save/Load Edge Cases

```
TC-020: Save during combat
  → Save allowed, enemy positions reset
  
TC-021: Load during combat
  → Combat ends, safe state
  
TC-022: Corrupted save file
  → Error message, fallback to new game
  
TC-023: Save while NPC in unusual state
  → State restored as-is
  
TC-024: Load with missing data (deleted mod?)
  → Use defaults, log warning
```

---

## 5. Narzędzia testowe

### 5.1 Unity Test Framework
- Unit tests for formulas
- Integration tests for data loading

### 5.2 Custom Debug Tools
```
DebugConsole (toggle with `):
- give_item [itemId]
- give_xp [amount]
- complete_quest [questId]
- set_flag [flagId]
- set_level [level]
- teleport [x y z]
- kill_all
- god_mode
```

### 5.3 Editor Tools
- Quest Graph Visualizer
- NPC Spawner
- Item Spawner
- Scene Validator

---

## 6. CI/CD

### 6.1 Build Pipeline
```
Push to branch → 
Automated tests → 
Build Windows x64 → 
Upload artifact → 
Notify on failure
```

### 6.2 Test Coverage Goals
| System | Coverage |
|--------|----------|
| JSON Loader | 100% |
| Damage Calculator | 100% |
| Quest Manager | 80% |
| Dialog Manager | 70% |
| Save/Load | 90% |
