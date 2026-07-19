# DATA_SCHEMAS.md — Schematy danych JSON

## 1. Struktura katalogów

```
Assets/StreamingAssets/
├── Data/
│   ├── Json/
│   │   ├── items/
│   │   │   ├── items_weapons_swords.json
│   │   │   ├── items_weapons_bows.json
│   │   │   ├── items_armors.json
│   │   │   ├── items_plants.json
│   │   │   ├── items_potions.json
│   │   │   ├── items_trophies.json
│   │   │   └── items_misc.json
│   │   │
│   │   ├── npcs/
│   │   │   ├── npcs.json
│   │   │   └── npc_schedules.json
│   │   │
│   │   ├── monsters/
│   │   │   ├── monsters.json
│   │   │   └── monster_spawns.json
│   │   │
│   │   ├── quests/
│   │   │   ├── quests_main.json
│   │   │   ├── quests_old_faction.json
│   │   │   ├── quests_new_faction.json
│   │   │   └── quests_side.json
│   │   │
│   │   ├── dialogues/
│   │   │   ├── dialogues_greeting.json
│   │   │   ├── dialogues_old_order.json
│   │   │   ├── dialogues_new_order.json
│   │   │   ├── dialogues_merchants.json
│   │   │   └── dialogues_misc.json
│   │   │
│   │   ├── world/
│   │   │   ├── world_locations.json
│   │   │   ├── loot_tables.json
│   │   │   └── world_objects.json
│   │   │
│   │   ├── gameplay/
│   │   │   ├── trainers.json
│   │   │   ├── spells.json
│   │   │   └── balance.json
│   │   │
│   │   └── saves/
│   │       └── example_savegame.json
│   │
│   └── Schemas/
│       ├── item_schema.json
│       ├── npc_schema.json
│       ├── quest_schema.json
│       ├── dialogue_schema.json
│       ├── world_schema.json
│       └── common_definitions.json
```

---

## 2. Wspólne definicje

### 2.1 ID Reference Format
- Wszystkie ID: `type_subtype_name` (snake_case)
- Prefixy: `item_`, `npc_`, `monster_`, `quest_`, `dialog_`, `spell_`, `location_`, `loot_`, `flag_`

### 2.2 Common Types

```json
{
  "definitions": {
    "itemId": {
      "type": "string",
      "pattern": "^item_[a-z_]+$",
      "example": "item_iron_sword_01"
    },
    "npcId": {
      "type": "string", 
      "pattern": "^npc_[a-z_]+$",
      "example": "npc_aldona"
    },
    "localizedText": {
      "type": "object",
      "properties": {
        "key": { "type": "string" },
        "fallback": { "type": "string" }
      }
    },
    "vec3": {
      "type": "object",
      "properties": {
        "x": { "type": "number" },
        "y": { "type": "number" },
        "z": { "type": "number" }
      }
    }
  }
}
```

---

## 3. Schematy przedmiotów (items)

### 3.1 Base Item Schema

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "$id": "item_schema.json",
  "type": "object",
  "definitions": {
    "baseItem": {
      "type": "object",
      "required": ["id", "nameKey", "itemType", "baseValue", "weight"],
      "properties": {
        "id": { "$ref": "#/definitions/itemId" },
        "nameKey": { "type": "string" },
        "descriptionKey": { "type": "string" },
        "itemType": {
          "enum": [
            "Weapon", "Armor", "Consumable", "Material", 
            "QuestItem", "Key", "Ammo", "Treasure"
          ]
        },
        "subType": { "type": "string" },
        "baseValue": { "type": "integer", "minimum": 0 },
        "weight": { "type": "number", "minimum": 0 },
        "iconPath": { "type": "string" },
        "modelPath": { "type": "string" },
        "stackable": { "type": "boolean", "default": false },
        "maxStack": { "type": "integer", "minimum": 1 },
        "isQuestItem": { "type": "boolean", "default": false },
        "discardable": { "type": "boolean", "default": true },
        "sellable": { "type": "boolean", "default": true },
        "priceModifier": { "type": "number", "default": 1.0 }
      }
    }
  }
}
```

### 3.2 Weapons (Swords)

```json
{
  "items": [
    {
      "id": "item_old_sword_01",
      "nameKey": "item_old_sword_01_name",
      "descriptionKey": "item_old_sword_01_desc",
      "itemType": "Weapon",
      "subType": "Sword",
      "baseValue": 15,
      "weight": 3.0,
      "iconPath": "Art/Icons/Weapons/old_sword",
      "modelPath": "Art/Models/Weapons/old_sword",
      "stackable": false,
      "damage": {
        "min": 8,
        "max": 12,
        "type": "Slashing"
      },
      "requirements": {
        "strength": 5,
        "dexterity": 0
      },
      "attackSpeed": 1.0,
      "reach": 1.5,
      "comboLevels": [1],
      "durability": {
        "max": 50,
        "current": 50
      }
    },
    {
      "id": "item_iron_sword_01",
      "nameKey": "item_iron_sword_01_name",
      "descriptionKey": "item_iron_sword_01_desc",
      "itemType": "Weapon",
      "subType": "Sword",
      "baseValue": 45,
      "weight": 4.0,
      "iconPath": "Art/Icons/Weapons/iron_sword",
      "modelPath": "Art/Models/Weapons/iron_sword",
      "stackable": false,
      "damage": {
        "min": 15,
        "max": 22,
        "type": "Slashing"
      },
      "requirements": {
        "strength": 8,
        "dexterity": 0
      },
      "attackSpeed": 1.1,
      "reach": 1.8,
      "comboLevels": [1, 2],
      "durability": {
        "max": 100,
        "current": 100
      }
    },
    {
      "id": "item_steel_sword_01",
      "nameKey": "item_steel_sword_01_name",
      "descriptionKey": "item_steel_sword_01_desc",
      "itemType": "Weapon",
      "subType": "Sword",
      "baseValue": 120,
      "weight": 5.0,
      "iconPath": "Art/Icons/Weapons/steel_sword",
      "modelPath": "Art/Models/Weapons/steel_sword",
      "stackable": false,
      "damage": {
        "min": 25,
        "max": 35,
        "type": "Slashing"
      },
      "requirements": {
        "strength": 12,
        "dexterity": 2
      },
      "attackSpeed": 1.2,
      "reach": 2.0,
      "comboLevels": [1, 2, 3],
      "durability": {
        "max": 150,
        "current": 150
      }
    }
  ]
}
```

### 3.3 Weapons (Bows)

```json
{
  "items": [
    {
      "id": "item_short_bow_01",
      "nameKey": "item_short_bow_01_name",
      "descriptionKey": "item_short_bow_01_desc",
      "itemType": "Weapon",
      "subType": "Bow",
      "baseValue": 35,
      "weight": 2.0,
      "iconPath": "Art/Icons/Weapons/short_bow",
      "modelPath": "Art/Models/Weapons/short_bow",
      "stackable": false,
      "damage": {
        "min": 10,
        "max": 14,
        "type": "Piercing"
      },
      "requirements": {
        "strength": 0,
        "dexterity": 6
      },
      "attackSpeed": 1.5,
      "range": 30.0,
      "drawTime": 0.8,
      "ammoType": "Arrow",
      "durability": {
        "max": 80,
        "current": 80
      }
    },
    {
      "id": "item_hunting_bow_01",
      "nameKey": "item_hunting_bow_01_name",
      "descriptionKey": "item_hunting_bow_01_desc",
      "itemType": "Weapon",
      "subType": "Bow",
      "baseValue": 80,
      "weight": 2.5,
      "iconPath": "Art/Icons/Weapons/hunting_bow",
      "modelPath": "Art/Models/Weapons/hunting_bow",
      "stackable": false,
      "damage": {
        "min": 18,
        "max": 25,
        "type": "Piercing"
      },
      "requirements": {
        "strength": 0,
        "dexterity": 10
      },
      "attackSpeed": 1.2,
      "range": 45.0,
      "drawTime": 1.0,
      "ammoType": "Arrow",
      "durability": {
        "max": 120,
        "current": 120
      }
    }
  ]
}
```

### 3.4 Armors

```json
{
  "items": [
    {
      "id": "item_cloth_robe_01",
      "nameKey": "item_cloth_robe_01_name",
      "descriptionKey": "item_cloth_robe_01_desc",
      "itemType": "Armor",
      "subType": "Cloth",
      "slot": "Body",
      "baseValue": 20,
      "weight": 1.5,
      "iconPath": "Art/Icons/Armors/cloth_robe",
      "modelPath": "Art/Models/Armors/cloth_robe",
      "stackable": false,
      "armor": {
        "physical": 2,
        "magical": 0,
        "fire": 0,
        "ice": 0,
        "lightning": 0
      },
      "requirements": {
        "strength": 0
      },
      "durability": {
        "max": 30,
        "current": 30
      }
    },
    {
      "id": "item_leather_armor_01",
      "nameKey": "item_leather_armor_01_name",
      "descriptionKey": "item_leather_armor_01_desc",
      "itemType": "Armor",
      "subType": "Leather",
      "slot": "Body",
      "baseValue": 60,
      "weight": 4.0,
      "iconPath": "Art/Icons/Armors/leather_armor",
      "modelPath": "Art/Models/Armors/leather_armor",
      "stackable": false,
      "armor": {
        "physical": 8,
        "magical": 0,
        "fire": 1,
        "ice": 1,
        "lightning": 0
      },
      "requirements": {
        "strength": 3
      },
      "durability": {
        "max": 60,
        "current": 60
      }
    },
    {
      "id": "item_chainmail_01",
      "nameKey": "item_chainmail_01_name",
      "descriptionKey": "item_chainmail_01_desc",
      "itemType": "Armor",
      "subType": "Chainmail",
      "slot": "Body",
      "baseValue": 150,
      "weight": 8.0,
      "iconPath": "Art/Icons/Armors/chainmail",
      "modelPath": "Art/Models/Armors/chainmail",
      "stackable": false,
      "armor": {
        "physical": 18,
        "magical": 0,
        "fire": 2,
        "ice": 2,
        "lightning": 1
      },
      "requirements": {
        "strength": 10
      },
      "durability": {
        "max": 120,
        "current": 120
      }
    },
    {
      "id": "item_plate_armor_01",
      "nameKey": "item_plate_armor_01_name",
      "descriptionKey": "item_plate_armor_01_desc",
      "itemType": "Armor",
      "subType": "Plate",
      "slot": "Body",
      "baseValue": 300,
      "weight": 15.0,
      "iconPath": "Art/Icons/Armors/plate_armor",
      "modelPath": "Art/Models/Armors/plate_armor",
      "stackable": false,
      "armor": {
        "physical": 35,
        "magical": 0,
        "fire": 5,
        "ice": 5,
        "lightning": 3
      },
      "requirements": {
        "strength": 18
      },
      "durability": {
        "max": 200,
        "current": 200
      }
    }
  ]
}
```

### 3.5 Plants

```json
{
  "items": [
    {
      "id": "item_herb_healing_01",
      "nameKey": "item_herb_healing_01_name",
      "descriptionKey": "item_herb_healing_01_desc",
      "itemType": "Consumable",
      "subType": "Plant",
      "baseValue": 5,
      "weight": 0.1,
      "iconPath": "Art/Icons/Plants/herb_healing",
      "stackable": true,
      "maxStack": 10,
      "effect": {
        "type": "Heal",
        "value": 20,
        "duration": 0,
        "cooldown": 5
      },
      "harvestable": true,
      "biome": "Forest"
    },
    {
      "id": "item_herb_mana_01",
      "nameKey": "item_herb_mana_01_name",
      "descriptionKey": "item_herb_mana_01_desc",
      "itemType": "Consumable",
      "subType": "Plant",
      "baseValue": 8,
      "weight": 0.1,
      "iconPath": "Art/Icons/Plants/herb_mana",
      "stackable": true,
      "maxStack": 10,
      "effect": {
        "type": "RestoreMana",
        "value": 25,
        "duration": 0,
        "cooldown": 5
      },
      "harvestable": true,
      "biome": "Swamp"
    }
  ]
}
```

### 3.6 Potions

```json
{
  "items": [
    {
      "id": "item_potion_health_small",
      "nameKey": "item_potion_health_small_name",
      "descriptionKey": "item_potion_health_small_desc",
      "itemType": "Consumable",
      "subType": "Potion",
      "baseValue": 15,
      "weight": 0.3,
      "iconPath": "Art/Icons/Potions/potion_health_small",
      "stackable": true,
      "maxStack": 10,
      "effect": {
        "type": "Heal",
        "value": 50,
        "duration": 0,
        "cooldown": 10
      }
    },
    {
      "id": "item_potion_health_large",
      "nameKey": "item_potion_health_large_name",
      "descriptionKey": "item_potion_health_large_desc",
      "itemType": "Consumable",
      "subType": "Potion",
      "baseValue": 50,
      "weight": 0.5,
      "iconPath": "Art/Icons/Potions/potion_health_large",
      "stackable": true,
      "maxStack": 5,
      "effect": {
        "type": "Heal",
        "value": 150,
        "duration": 0,
        "cooldown": 30
      }
    }
  ]
}
```

---

## 4. Schemat NPC

```json
{
  "npcs": [
    {
      "id": "npc_aldona",
      "nameKey": "npc_aldona_name",
      "titleKey": "npc_aldona_title",
      "role": "Leader",
      "faction": "OldOrder",
      "level": 15,
      "isEssential": true,
      "defaultGreeting": "dialog_aldona_greeting",
      
      "stats": {
        "health": 200,
        "mana": 50,
        "strength": 20,
        "dexterity": 12,
        "armor": 25,
        "damage": {
          "min": 15,
          "max": 25
        }
      },
      
      "equipment": [
        { "slot": "Weapon", "itemId": "item_steel_sword_01" },
        { "slot": "Armor", "itemId": "item_chainmail_01" }
      ],
      
      "inventory": [
        { "itemId": "gold", "count": 100 },
        { "itemId": "item_iron_key_01", "count": 1 }
      ],
      
      "initialRelation": 0,
      "scheduleId": "npc_aldona_schedule",
      
      "dialogues": ["dialog_aldona_greeting", "dialog_aldona_quest_001"],
      "quests": ["Q_F_OLD_001", "Q_F_OLD_002", "Q_F_OLD_FINAL"],
      "trainer": null,
      
      "reactions": {
        "theft": "AlertAndFight",
        "trespass": "Warning",
        "attack": "FleeAndAlert"
      },
      
      "visual": {
        "modelPath": "Art/Models/NPCs/Aldona",
        "skinTone": "#D4A574",
        "hairColor": "#4A3728",
        "clothesColor": "#8B0000"
      },
      
      "voice": {
        "pitch": 0.9,
        "volume": 1.0
      },
      
      "location": {
        "scene": "World_KamiennyBrzeg",
        "position": { "x": 10.0, "y": 0.0, "z": 5.0 },
        "rotation": { "x": 0.0, "y": 180.0, "z": 0.0 }
      }
    }
  ]
}
```

---

## 5. Schemat NPC Schedules

```json
{
  "schedules": [
    {
      "npcId": "npc_aldona",
      "phases": [
        {
          "timeRange": { "start": 6, "end": 8 },
          "locationId": "location_aldona_chambers",
          "activity": "Sleep",
          "animation": "sleep_idle",
          "conditions": [],
          "fallback": { "position": { "x": 10, "y": 0, "z": 5 } }
        },
        {
          "timeRange": { "start": 8, "end": 10 },
          "locationId": "location_aldona_chambers",
          "activity": "Work",
          "animation": "reading",
          "conditions": [],
          "fallback": { "position": { "x": 10, "y": 0, "z": 5 } }
        },
        {
          "timeRange": { "start": 10, "end": 14 },
          "locationId": "location_dwor_main",
          "activity": "Social",
          "animation": "sitting_talk",
          "conditions": [],
          "fallback": { "position": { "x": 0, "y": 0, "z": 0 } }
        },
        {
          "timeRange": { "start": 14, "end": 18 },
          "locationId": "location_dwor_main",
          "activity": "Work",
          "animation": "administrating",
          "conditions": [],
          "fallback": { "position": { "x": 0, "y": 0, "z": 0 } }
        },
        {
          "timeRange": { "start": 18, "end": 20 },
          "locationId": "location_dwor_dining",
          "activity": "Eat",
          "animation": "eating",
          "conditions": [],
          "fallback": { "position": { "x": -5, "y": 0, "z": 3 } }
        },
        {
          "timeRange": { "start": 20, "end": 22 },
          "locationId": "location_dwor_main",
          "activity": "Social",
          "animation": "standing_talk",
          "conditions": [],
          "fallback": { "position": { "x": 0, "y": 0, "z": 0 } }
        },
        {
          "timeRange": { "start": 22, "end": 6 },
          "locationId": "location_aldona_chambers",
          "activity": "Sleep",
          "animation": "sleep_idle",
          "conditions": [],
          "fallback": { "position": { "x": 10, "y": 0, "z": 5 } }
        }
      ],
      "overrides": [
        {
          "condition": { "type": "QuestActive", "questId": "Q_F_OLD_001" },
          "phases": [
            {
              "timeRange": { "start": 10, "end": 18 },
              "locationId": "location_dwor_main",
              "activity": "Work",
              "animation": "waiting",
              "conditions": [],
              "fallback": { "position": { "x": 0, "y": 0, "z": 0 } }
            }
          ]
        }
      ]
    }
  ]
}
```

---

## 6. Schemat Monsters

```json
{
  "monsters": [
    {
      "id": "monster_wolf",
      "nameKey": "monster_wolf_name",
      "descriptionKey": "monster_wolf_desc",
      
      "type": "Animal",
      "biome": ["Forest", "Mountain"],
      "behaviour": "Pack",
      
      "stats": {
        "health": 40,
        "strength": 8,
        "dexterity": 14,
        "armor": 2,
        "damage": {
          "min": 8,
          "max": 12,
          "type": "Slashing"
        },
        "attackSpeed": 1.3,
        "moveSpeed": 5.0
      },
      
      "lootTable": "loot_wolf",
      "trophyItem": "item_wolf_pelt",
      "trophyCount": 1,
      "skinReward": {
        "itemId": "item_wolf_pelt",
        "minCount": 1,
        "maxCount": 2,
        "requiresSkill": "skinNpe",
        "skillLevel": 1
      },
      
      "detection": {
        "sight": 15.0,
        "hearing": 20.0,
        "alertRange": 5.0,
        "chaseRange": 30.0
      },
      
      "rewards": {
        "xp": 50,
        "gold": 5
      },
      
      "isHostile": true,
      "isNocturnal": false,
      "isBoss": false,
      
      "visual": {
        "modelPath": "Art/Models/Creatures/Wolf",
        "scale": 1.0,
        "iconPath": "Art/Icons/Creatures/wolf"
      }
    },
    {
      "id": "monster_troll",
      "nameKey": "monster_troll_name",
      "descriptionKey": "monster_troll_desc",
      
      "type": "Monster",
      "biome": ["Mountain", "Cave"],
      "behaviour": "Territorial",
      
      "stats": {
        "health": 400,
        "strength": 35,
        "dexterity": 4,
        "armor": 15,
        "damage": {
          "min": 35,
          "max": 55,
          "type": "Blunt"
        },
        "attackSpeed": 0.6,
        "moveSpeed": 2.5
      },
      
      "lootTable": "loot_troll",
      "trophyItem": "item_troll_heart",
      "trophyCount": 1,
      "skinReward": null,
      
      "detection": {
        "sight": 20.0,
        "hearing": 15.0,
        "alertRange": 8.0,
        "chaseRange": 50.0
      },
      
      "rewards": {
        "xp": 500,
        "gold": 150
      },
      
      "isHostile": true,
      "isNocturnal": false,
      "isBoss": true,
      "respawnTime": 3600,
      
      "visual": {
        "modelPath": "Art/Models/Creatures/Troll",
        "scale": 2.5,
        "iconPath": "Art/Icons/Creatures/troll"
      }
    }
  ]
}
```

---

## 7. Schemat Quests

```json
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "$id": "quest_schema.json",
  
  "quest": {
    "type": "object",
    "required": ["id", "titleKey", "descriptionKey", "type", "requirements", "stages", "rewards"],
    "properties": {
      "id": {
        "type": "string",
        "pattern": "^Q_[A-Z]_[0-9]{3}$"
      },
      "titleKey": { "type": "string" },
      "descriptionKey": { "type": "string" },
      "type": {
        "enum": ["Main", "FactionOld", "FactionNew", "Side", "Hidden"]
      },
      "faction": {
        "enum": ["None", "OldOrder", "NewOrder", "Bandits"]
      },
      "requirements": {
        "type": "object",
        "properties": {
          "level": { "type": "integer", "minimum": 0 },
          "quests": {
            "type": "array",
            "items": { "type": "string" }
          },
          "flags": {
            "type": "array", 
            "items": { "type": "string" }
          },
          "items": {
            "type": "array",
            "items": { "type": "string" }
          },
          "skills": {
            "type": "object",
            "additionalProperties": { "type": "integer" }
          }
        }
      },
      "stages": {
        "type": "array",
        "minItems": 1,
        "items": {
          "type": "object",
          "required": ["id", "descriptionKey", "objectives"],
          "properties": {
            "id": { "type": "integer" },
            "descriptionKey": { "type": "string" },
            "objectives": {
              "type": "array",
              "items": {
                "type": "object",
                "required": ["type", "description"],
                "properties": {
                  "type": {
                    "enum": ["Talk", "Kill", "Collect", "Deliver", "Explore", "Escort", "Persuade", "Steal", "Interact", "Craft"]
                  },
                  "target": { "type": "string" },
                  "count": { "type": "integer" },
                  "itemId": { "type": "string" },
                  "description": { "type": "string" },
                  "optional": { "type": "boolean", "default": false },
                  "failOnDeath": { "type": "boolean", "default": true }
                }
              }
            },
            "onComplete": {
              "type": "object",
              "properties": {
                "addFlags": { "type": "array", "items": { "type": "string" } },
                "removeFlags": { "type": "array", "items": { "type": "string" } },
                "giveItems": { "type": "array", "items": { "type": "string" } },
                "removeItems": { "type": "array", "items": { "type": "string" } },
                "startQuest": { "type": "string" },
                "failQuest": { "type": "string" },
                "changeReputation": {
                  "type": "object",
                  "additionalProperties": { "type": "integer" }
                },
                "changeRelation": {
                  "type": "object",
                  "additionalProperties": { "type": "integer" }
                },
                "unlockDialogue": { "type": "array", "items": { "type": "string" } },
                "unlockTrainer": { "type": "string" },
                "playCutscene": { "type": "string" },
                "teleport": {
                  "type": "object",
                  "properties": {
                    "scene": { "type": "string" },
                    "position": { "$ref": "#/definitions/vec3" }
                  }
                }
              }
            },
            "onFail": {
              "type": "object",
              "properties": {
                "failQuest": { "type": "boolean" },
                "addFlags": { "type": "array", "items": { "type": "string" } }
              }
            }
          }
        }
      },
      "rewards": {
        "type": "object",
        "properties": {
          "xp": { "type": "integer" },
          "gold": { "type": "integer" },
          "items": {
            "type": "array",
            "items": { "type": "string" }
          },
          "flags": {
            "type": "array",
            "items": { "type": "string" }
          },
          "skillPoints": { "type": "integer" },
          "reputation": {
            "type": "object",
            "additionalProperties": { "type": "integer" }
          }
        }
      },
      "failConditions": {
        "type": "array",
        "items": {
          "type": "object",
          "properties": {
            "type": { "type": "string" },
            "target": { "type": "string" }
          }
        }
      },
      "parentQuest": { "type": "string" },
      "nextQuest": { "type": "string" },
      "cancelOnFail": { "type": "boolean", "default": true }
    }
  }
}
```

---

## 8. Schemat Dialogue

```json
{
  "dialogueTree": {
    "type": "object",
    "required": ["rootNodeId", "nodes"],
    "properties": {
      "id": { "type": "string" },
      "rootNodeId": { "type": "string" },
      "speaker": { "type": "string" },
      "nodes": {
        "type": "object",
        "additionalProperties": {
          "$ref": "#/definitions/dialogueNode"
        }
      }
    }
  },
  
  "dialogueNode": {
    "type": "object",
    "required": ["nodeId", "text", "choices"],
    "properties": {
      "nodeId": { "type": "string" },
      "speaker": { "type": "string" },
      "text": { "type": "string" },
      "portrait": { "type": "string" },
      "animation": { "type": "string" },
      "choices": {
        "type": "array",
        "items": {
          "type": "object",
          "required": ["text"],
          "properties": {
            "text": { "type": "string" },
            "conditions": {
              "type": "array",
              "items": {
                "$ref": "#/definitions/dialogueCondition"
              }
            },
            "actions": {
              "type": "array",
              "items": {
                "$ref": "#/definitions/dialogueAction"
              }
            },
            "nextNode": { "type": "string" },
            "failNode": { "type": "string" },
            "requiredSkill": { "type": "string" },
            "skillCheckDC": { "type": "integer" },
            "requiredStat": { "type": "string" },
            "statCheckDC": { "type": "integer" }
          }
        }
      },
      "onEnter": {
        "type": "array",
        "items": { "$ref": "#/definitions/dialogueAction" }
      },
      "onExit": {
        "type": "array",
        "items": { "$ref": "#/definitions/dialogueAction" }
      }
    }
  },
  
  "dialogueCondition": {
    "type": "object",
    "required": ["type"],
    "properties": {
      "type": {
        "enum": ["HasItem", "HasFlag", "QuestActive", "QuestComplete", "QuestFailed", 
                 "LevelGte", "ReputationGte", "SkillGte", "StatGte", "TimeOfDay", "Random", 
                 "FactionMatches", "IsMale", "IsFemale"]
      },
      "itemId": { "type": "string" },
      "flag": { "type": "string" },
      "questId": { "type": "string" },
      "value": { "type": "integer" },
      "stat": { "type": "string" },
      "faction": { "type": "string" },
      "probability": { "type": "number", "minimum": 0, "maximum": 100 }
    }
  },
  
  "dialogueAction": {
    "type": "object",
    "required": ["type"],
    "properties": {
      "type": {
        "enum": ["StartQuest", "CompleteQuestStage", "FailQuest", "SetFlag", "ClearFlag",
                 "GiveItem", "RemoveItem", "ChangeRelation", "ChangeReputation",
                 "Teleport", "PlayAnimation", "AddShop", "UnlockTrainer", "LearnSpell",
                 "OpenShop", "EndDialogue", "PlaySound", "ChangeWorldState"]
      },
      "questId": { "type": "string" },
      "stageId": { "type": "integer" },
      "flag": { "type": "string" },
      "itemId": { "type": "string" },
      "count": { "type": "integer" },
      "value": { "type": "integer" },
      "scene": { "type": "string" },
      "position": { "$ref": "#/definitions/vec3" },
      "animation": { "type": "string" },
      "sound": { "type": "string" },
      "trainerId": { "type": "string" },
      "spellId": { "type": "string" }
    }
  }
}
```

---

## 9. Walidacja

### Zasady walidacji:
1. Wszystkie ID muszą być unikalne w swoim typie
2. Referencje między plikami muszą istnieć
3. Wymagania questów muszą być spełnialne
4. Dialogi muszą mieć zakończenie (node bez nextNode)
5. NPC schedules muszą pokrywać całą dobę (0-24)

### Walidator uruchamiany:
1. W edytorze Unity (EditorWindow)
2. Jako test w CI/CD
3. Przy starcie gry (warning only)

---

## 10. Migracja wersji

Gdy schemat się zmienia:
1. Zwiększ `schemaVersion`
2. Dodaj migration script w `DataMigration.cs`
3. Napisz test migracji
4. Zapisz starą wersję jako `deprecated`

```json
{
  "migrationRules": [
    {
      "fromVersion": "1.0.0",
      "toVersion": "1.1.0",
      "transformations": [
        { "type": "RenameField", "from": "oldName", "to": "newName" },
        { "type": "AddField", "field": "newField", "default": "value" },
        { "type": "RemoveField", "field": "oldField" },
        { "type": "ConvertType", "field": "field", "from": "int", "to": "string" }
      ]
    }
  ]
}
```
