# AUDIT_CHECKLIST.md

Audyt: 2026-07-19. Projekt rozpoznany jako **Unity 6 RPG 3D**, nie Godot 4. Nie migrowano stosu.

| ID | Wymaganie | Status | Dowód | Test/komenda | Problem | Następna akcja |
|---|---|---|---|---|---|---|
| A-01 | Unity project/scena startowa | PARTIAL | `ProjectVersion.txt`, `Assets/Scenes/VerticalSlice.unity` | `find`, inspekcja | Unity CLI niedostępne; scena greybox | Uruchomić Unity i naprawić ewentualne import errors |
| A-02 | JSON questów | PASS | 19 questów, 59 etapów, 59 celów | `python3 Tools/validate_quests.py` | brak | Dodać test referencji wszystkich kategorii |
| A-03 | Minimalny vertical slice | PARTIAL | `VerticalSliceBootstrap.cs`, `VerticalSlicePlayer.cs`, `VerticalSliceWorld.cs` | inspekcja kodu | ruch/dialog/quest/walka są proceduralne, bez realnego runtime testu | Test Play Mode |
| A-04 | 2 frakcje i finał | PARTIAL | `quests_*_faction.json`, `Q_M_010` | audyt JSON | brak zintegrowanego wyboru i blokady ścieżki | Implementacja runtime |
| A-05 | NPC/rutyny | FAIL | `npcs.json` (9), `npc_schedules.json` (5) | zliczenie danych | wymagane ~65 NPC; brak działającego AI/rutyn | Rozszerzyć dane i system |
| A-06 | Itemy | FAIL | swords=8, bows=5, armors=6 | skrypt zliczający | minima 20/10 nieosiągnięte; brak asset paths/modeli | Uzupełnić dane i walidację |
| A-07 | Potwory | PARTIAL | `monsters.json` (6) | zliczenie | dane są, brak sześciu zintegrowanych zachowań/spawnów | Implementacja AI/spawnów |
| A-08 | Zapis/wczytanie | PARTIAL | `SaveDataManager` | inspekcja | brak testu silnikowego i sceny UI | Testy i integracja |
| A-09 | Build/testy Unity | BLOCKED | brak binarki `unity` | `command -v unity` | środowisko nie ma Unity | Uruchomić w Unity Editor/CI |
| A-10 | Dokumentacja zgodna ze stanem | PARTIAL | `PROGRESS.md`, `TODO.md` | porównanie | część statusów deklaruje systemy jako ukończone bez builda | Aktualizacja po audycie |
| A-11 | Assety/licencje | PARTIAL | `THIRD_PARTY_ASSETS.md` | inspekcja | brak finalnych assetów; tylko proceduralny greybox | Dodać własne assety albo oznaczyć TODO |
| A-12 | Godot 4 | NOT_TESTED | brak plików Godot | inspekcja | wymaganie promptu dotyczy wariantu Godot, ale repo jest Unity | Audytować faktyczny Unity wariant |
