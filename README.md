# Żelazna Droga — Gra Action RPG 3D

## Opis
**Żelazna Droga** to samodzielna gra action RPG 3D osadzona w mrocznym, surowym świecie inspirowanym klasycznymi europejskimi RPG. Gracz wciela się w nikomu nieznanego przybysza, który musi przetrwać w skonfliktowanej krainie, wybierając między porządkiem starej gildii a wolnością rebelii.

## Wymagania
- **Silnik:** Unity 6 LTS
- **Język:** C# (.NET 7+ dla kompilacji offline)
- **API Grafiki:** Universal Render Pipeline (URP)
- **Platforma:** Windows (x64)
- **Sterowanie:** Klawiatura + Mysz

## Sterowanie

### Ruch
| Akcja | Klawisz |
|-------|---------|
| Ruch | WASD |
| Bieg | Shift (przytrzymaj) |
| Skok | Spacja |
| Obrót kamery | Mysz |
| Przyspieszenie czasu (sen) | T |

### Interakcje
| Akcja | Klawisz |
|-------|---------|
| Interakcja / Dialog | E |
| Menu ekwipunku | I |
| Dziennik zadań | J |
| Karta postaci | C |
| Pauza | Escape |

### Walka
| Akcja | Klawisz |
|-------|---------|
| Lekki atak | LPM |
| Mocny atak | PPM |
| Blok/Unik | Prawy przycisk |
| Dobycie/włożenie broni | R |
| Wybór broni 1/2/3 | 1 / 2 / 3 |
| Magiczny pocisk | F |
| Leczniczy czar | G |

### Kradzież i skrzynie
| Akcja | Klawisz |
|-------|---------|
| Próba kradzieży | E (przy NPC) |
| Otwórz zamek | E (przy skrzyni) |
| Minigra zamka | Lewo/Prawo strzałki |

## Struktura projektu
```
gothic-unity/
├── Assets/
│   ├── Scripts/           # Kod C# (Assembly Definitions)
│   ├── StreamingAssets/   # Dane JSON
│   ├── Prefabs/           # Prefaby
│   ├── Scenes/            # Sceny
│   ├── Art/               # Assety graficzne
│   └── Audio/             # Dźwięki
├── ProjectSettings/       # Ustawienia Unity
└── Packages/
```

## Budowanie
```bash
# W Unity Editor
# File → Build Settings → Windows x64 → Build

# Lub z командной строки (wymaga Unity CLI)
unity -batchmode -quit -projectPath . -buildWindows64Player -outputPath Builds/Build.exe
```

## Architektura
- **Assembly Definitions:** Modularna architektura z osobnymi assembly dla systemów (Core, Gameplay, UI, Data)
- **Data-Driven:** Wszystkie dane stałe w JSON
- **ScriptableObjects:** Runtime reprezentacje danych
- **ECS-lite:** Komponenty i systemy dla wydajności

## Licencja
© 2024–2026. Wszystkie prawa zastrzeżone.
