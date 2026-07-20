# ARCHITECTURE.md — Architektura projektu

## 1. Assembly Definitions

Projekt podzielony na 5 głównych assembly dla modularności i czystości kodu:

```
gothic-unity/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/                    # Zależności podstawowe
│   │   │   ├── AssemblyDefinition.asmdef
│   │   │   ├── Attributes/
│   │   │   ├── Collections/
│   │   │   ├── Extensions/
│   │   │   └── Utilities/
│   │   │
│   │   ├── Data/                    # System danych i JSON
│   │   │   ├── AssemblyDefinition.asmdef
│   │   │   ├── Json/
│   │   │   ├── Schema/
│   │   │   ├── SoImporters/
│   │   │   └── Data.asmdef
│   │   │
│   │   ├── Gameplay/               # Logika gry
│   │   │   ├── AssemblyDefinition.asmdef
│   │   │   ├── Player/
│   │   │   ├── Combat/
│   │   │   ├── Quests/
│   │   │   ├── Dialogues/
│   │   │   ├── NPCs/
│   │   │   ├── Inventory/
│   │   │   ├── Skills/
│   │   │   ├── Progression/
│   │   │   ├── World/
│   │   │   └── Gameplay.asmdef
│   │   │
│   │   ├── AI/                     # Sztuczna inteligencja
│   │   │   ├── AssemblyDefinition.asmdef
│   │   │   ├── Navigation/
│   │   │   ├── Behavior/
│   │   │   └── AI.asmdef
│   │   │
│   │   └── UI/                     # Interfejs użytkownika
│   │       ├── AssemblyDefinition.asmdef
│   │       ├── HUD/
│   │       ├── Inventory/
│   │       ├── Journal/
│   │       ├── Dialogues/
│   │       ├── Menus/
│   │       └── UI.asmdef
```

### Zależności między assembly:
```
Core ← Data ← Gameplay ← AI, UI
     ↑
     └── (UI potrzebuje Data dla schematów)
```

---

## 2. Namespace'y

```csharp
namespace ZelaznaDroga.Core           // Podstawowe narzędzia
namespace ZelaznaDroga.Data           // Dane i serializacja
namespace ZelaznaDroga.Data.Schema    // Schematy JSON
namespace ZelaznaDroga.Gameplay       // Logika gameplay
namespace ZelaznaDroga.Gameplay.Player
namespace ZelaznaDroga.Gameplay.Combat
namespace ZelaznaDroga.Gameplay.Quests
namespace ZelaznaDroga.Gameplay.Dialogues
namespace ZelaznaDroga.Gameplay.NPCs
namespace ZelaznaDroga.Gameplay.Inventory
namespace ZelaznaDroga.Gameplay.Skills
namespace ZelaznaDroga.Gameplay.Progression
namespace ZelaznaDroga.Gameplay.World
namespace ZelaznaDroga.AI
namespace ZelaznaDroga.AI.Navigation
namespace ZelaznaDroga.AI.Behavior
namespace ZelaznaDroga.UI
namespace ZelaznaDroga.UI.HUD
namespace ZelaznaDroga.UI.Inventory
namespace ZelaznaDroga.UI.Journal
namespace ZelaznaDroga.UI.Dialogues
namespace ZelaznaDroga.UI.Menus
```

---

## 3. Główne komponenty i systemy

### 3.1 Core Components

| Komponent | Opis | Assembly |
|-----------|------|----------|
| `MonoBehaviour` base classes | BaseMB, Singleton | Core |
| `ScriptableObject` base classes | BaseSO, DatabaseSO | Core |
| `EventBus` | Pub/sub messaging | Core |
| `ComponentLocator` | Service locator | Core |
| `Scheduler` | Delayed/periodic tasks | Core |

### 3.2 Data Layer

| Komponent | Opis | Assembly |
|-----------|------|----------|
| `JsonDataLoader` | Ładowanie JSON do runtime objects | Data |
| `DataValidator` | Walidacja schematów | Data |
| `ItemDatabase` | Reprezentacja przedmiotów | Data |
| `NpcDatabase` | Reprezentacja NPC | Data |
| `QuestDatabase` | Reprezentacja questów | Data |
| `DialogueDatabase` | Reprezentacja dialogów | Data |

### 3.3 Gameplay Components

| System | Opis | Assembly |
|--------|------|----------|
| `PlayerController` | Sterowanie postacią | Gameplay |
| `PlayerInventory` | Ekwipunek gracza | Gameplay |
| `PlayerStats` | Statystyki gracza | Gameplay |
| `SkillSystem` | Umiejętności | Gameplay |
| `QuestManager` | Zarządzanie questami | Gameplay |
| `DialogueManager` | Dialogi | Gameplay |
| `NpcBrain` | Logika NPC | Gameplay |
| `CombatSystem` | Walka (sieć systemów) | Gameplay |

### 3.4 AI Components

| Komponent | Opis | Assembly |
|-----------|------|----------|
| `NavMeshController` | Nawigacja | AI |
| `StateMachine` | Maszyna stanów | AI |
| `PatrolBehavior` | Patrol NPC | AI |
| `AlertBehavior` | Reakcja na gracza | AI |
| `CombatBehavior` | AI walki | AI |

### 3.5 UI Components

| Komponent | Opis | Assembly |
|-----------|------|----------|
| `HUDSystem` | Wyświetlanie HUD | UI |
| `InventoryUI` | Ekwipunek | UI |
| `JournalUI` | Dziennik | UI |
| `DialogueUI` | Dialogi | UI |
| `MenuSystem` | Menu główne, pauza | UI |

---

## 4. Przepływ danych (Data Flow)

### 4.1 Ładowanie danych

```
JSON Files (StreamingAssets)
        │
        ▼
JsonDataLoader ──► Validacja ──► DataValidator
        │
        ▼
ScriptableObject Representations (Runtime)
        │
        ├─────────────────┐
        ▼                 ▼
   ItemDatabase     NpcDatabase
        │                 │
        ▼                 ▼
   GameSystems ←────┴────► UI
```

### 4.2 Zapis stanu gry

```
Runtime State (C# objects)
        │
        ▼
SaveGameSerializer
        │
        ▼
JSON (persistent path)
        │
        ▼
LoadGameSerializer
        │
        ▼
Runtime State (C# objects)
```

### 4.3 Przepływ walki

```
Input System
    │
    ▼
CombatInputHandler
    │
    ▼
WeaponController ──► Animation ──► HitDetection
    │                     │
    │                     ▼
    │              CombatFX (VFX)
    │
    ▼
DamageCalculator
    │
    ▼
TargetStats ──► HealthComponent
```

---

## 5. Schemat zapisu gry

```json
{
  "schemaVersion": "1.0.0",
  "saveVersion": "2024.1.0",
  "timestamp": "2024-01-15T14:30:00Z",
  "playtimeSeconds": 3600,
  
  "player": {
    "id": "player_001",
    "name": "Nieznajomy",
    "level": 5,
    "xp": 1250,
    "learningPoints": 3,
    
    "stats": {
      "strength": 12,
      "dexterity": 10,
      "mana": 8,
      "maxHealth": 100,
      "currentHealth": 85,
      "maxMana": 40,
      "currentMana": 35,
      "armor": 15
    },
    
    "position": {
      "x": 100.5,
      "y": 2.0,
      "z": -50.3
    },
    "rotation": {
      "x": 0,
      "y": 45.0,
      "z": 0
    },
    "currentScene": "World_KamiennyBrzeg"
  },
  
  "equipment": {
    "weapon1": "item_steel_sword_01",
    "weapon2": "item_hunting_bow_01",
    "armor": "item_leather_armor_01",
    "helmet": null,
    "boots": null,
    "gloves": null,
    "amulet": null
  },
  
  "inventory": [
    { "id": "item_herb_healing_01", "count": 5 },
    { "id": "item_iron_key_01", "count": 1 },
    { "id": "gold", "count": 250 }
  ],
  
  "skills": {
    "swordCombat": 2,
    "bowCombat": 1,
    "lockpicking": 1,
    "stealth": 0,
    "skinNpe": 1,
    "magicFireball": 1,
    "magicHeal": 0
  },
  
  "quests": {
    "active": [
      {
        "id": "Q_F_OLD_001",
        "stageIndex": 2,
        "stageObjectives": {
          "wolfKillCount": 2
        },
        "startTime": "..."
      }
    ],
    "completed": ["Q_M_001", "Q_M_002"],
    "failed": []
  },
  
  "dialogueFlags": {
    "met_aldona": true,
    "aldona_trust": false,
    "refused_old_order": false
  },
  
  "reputation": {
    "oldOrder": 15,
    "newOrder": -5,
    "bandits": 0
  },
  
  "npcStates": [
    {
      "id": "npc_boruk",
      "currentState": "patrolling",
      "relation": 5,
      "isAlive": true,
      "equipment": ["item_iron_sword_01"],
      "inventory": []
    },
    {
      "id": "npc_farmer_kowalski",
      "currentState": "dead",
      "relation": 0,
      "isAlive": false,
      "killedBy": "player"
    }
  ],
  
  "worldState": {
    "chestsOpened": ["chest_forest_01", "chest_cave_03"],
    "itemsTaken": ["item_hidden_gold_01"],
    "doorsOpened": ["door_mine_01"],
    "objectsDestroyed": [],
    "weatherOverride": null
  },
  
  "factions": {
    "chosenFaction": null,
    "oldOrderQuests": 2,
    "newOrderQuests": 0,
    "betrayedOldOrder": false,
    "betrayedNewOrder": false
  },
  
  "statistics": {
    "enemiesKilled": 23,
    "questsCompleted": 5,
    "itemsCrafted": 0,
    "chestsOpened": 8,
    "locksPicked": 3,
    "itemsStolen": 1,
    "deaths": 2,
    "distanceTraveled": 15000
  }
}
```

---

## 6. Prefaby (Key Prefabs)

```
Assets/Prefabs/
├── Player/
│   ├── Player_Prefab.prefab        # Główny prefab gracza
│   ├── Player_Visual.prefab        # Model + animacje
│   └── Player_Weapon*.prefab       # Bronie gracza
│
├── NPCs/
│   ├── NPC_Template.prefab          # Bazowy NPC
│   ├── OldOrder/
│   │   ├── Aldona.prefab
│   │   ├── Boruk.prefab
│   │   └── ...
│   └── NewOrder/
│       ├── Drwal.prefab
│       └── ...
│
├── Enemies/
│   ├── Wolf_Prefab.prefab
│   ├── Bear_Prefab.prefab
│   ├── Bandit_Prefab.prefab
│   └── ...
│
├── Interactables/
│   ├── Chest_Template.prefab
│   ├── Door_Template.prefab
│   ├── Loot_Pile.prefab
│   └── ...
│
├── UI/
│   ├── HUD_Canvas.prefab
│   ├── Inventory_Panel.prefab
│   ├── Dialogue_Panel.prefab
│   └── Journal_Panel.prefab
│
└── World/
    ├── Campfire.prefab
    ├── Barrier.prefab
    └── ...
```

---

## 7. Sceny

```
Assets/Scenes/
├── Boot/
│   └── BootScene.unity              # Inicjalizacja
├── World/
│   ├── World_Main.unity             # Główny świat
│   └── World_Loading.unity         # Ekran ładowania
├── Menus/
│   └── MainMenu.unity              # Menu główne
└── Test/
    ├── Test_Combat.unity
    ├── Test_Inventory.unity
    └── Test_Dialogue.unity
```

---

## 8. State Machines

### 8.1 Player States
```
Idle → Walk → Run → Jump → Fall → Land
       ↑
       └── CombatMode:
           Idle → AttackLight/AttackHeavy → HitReaction → Death
           ↑
           Blocking → BlockHit → BlockBreak
           ↑
           Dodge
```

### 8.2 NPC States
```
Sleep → Wake → Idle → Work/Patrol → Eat → Social → Idle
       ↑
       └── Alerted:
           Investigate → Chase → Attack → Search → Return
           ↑
           Caught:
               Flee → CallForHelp
               ↑
               Confront:
                   Talk → Persuade/Fight
```

### 8.3 Combat AI States
```
Idle → Patrol → Alert → Investigate → Combat → 
       ↑                                          ↓
       └────────────── Return ← Search ← Retreat ←─┘
```

---

## 9. Event System

### Globalne eventy (EventBus):
```csharp
// Game Events
Events.Player.HealthChanged(int current, int max)
Events.Player.ManaChanged(int current, int max)
Events.Player.LevelUp(int newLevel)
Events.Player.Died()
Events.Player.Respawned()
Events.Player.EquippedItem(ItemInstance item)

// Quest Events
Events.Quest.Started(string questId)
Events.Quest.StageCompleted(string questId, int stage)
Events.Quest.Completed(string questId)
Events.Quest.Failed(string questId)

// World Events
Events.NPC.Killed(string npcId, string killerId)
Events.NPC.Interacted(string npcId)
Events.World.ChestOpened(string chestId)
Events.World.ItemTaken(string itemId)
Events.World.DoorOpened(string doorId)
Events.World.CrimeWitnessed(string witnessId, CrimeType type)

// Faction Events
Events.Faction.ReputationChanged(string factionId, int delta)
Events.Faction.Chosen(string factionId)
```

---

## 10. Dependency Injection

### Service Locator (ComponentLocator):
```csharp
public static class Services
{
    public static IDataLoader DataLoader { get; set; }
    public static IPlayerController Player { get; set; }
    public static IQuestManager Quests { get; set; }
    public static IDialogueManager Dialogues { get; set; }
    public static IInventoryManager Inventory { get; set; }
    public static ICombatSystem Combat { get; set; }
    public static INpcManager Npcs { get; set; }
    public static ISaveSystem SaveLoad { get; set; }
    public static ITimeManager Time { get; set; }
}
```

Użycie:
```csharp
public class SomeSystem : MonoBehaviour
{
    [Inject] private IPlayerController _player;
    [Inject] private IQuestManager _quests;
    
    private void Start()
    {
        Services.InjectDependencies(this);
    }
}
```
