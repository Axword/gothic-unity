# BUGS.md

## Aktywne

| ID | Priorytet | Problem | Reprodukcja/dowód | Rozwiązanie |
|---|---|---|---|---|
| AUD-001 | P0 | Nie można wykonać builda ani Play Mode w środowisku | `command -v unity` nie zwraca narzędzia | Uruchomić Unity 6/CI; nie oznaczać jako PASS |
| AUD-002 | P1 | Questy z JSON nie są zintegrowane z greyboxowym quest flow | `QuestManager` ładuje dane, ale Bootstrap używa `VerticalSliceQuest` | Podłączyć runtime do `QuestManager` i testować przejścia |
| AUD-003 | P1 | Brak wymaganych minimów itemów i NPC | 8/20 mieczy, 5/10 łuków, 9/~65 NPC | Uzupełnić dane kanoniczne i walidator minimów |
| AUD-004 | P1 | Brak 10 questów pobocznych | `quests_side.json`: 3 rekordy | Dodać 7 oryginalnych questów |
| AUD-005 | P2 | Proceduralne placeholdery zamiast finalnych modeli/animacji | `ProceduralModelFactory.cs` | Zastąpić własnymi assetami lub jawnie utrzymać greybox |

## Zamknięte

| ID | Naprawa | Dowód |
|---|---|---|
| AUD-006 | Loader rozpakowuje koperty JSON do `List<T>` | `JsonDataLoader.cs`, commit `ab05f2b` |
| AUD-007 | Walidator questów wykrywa duplikaty, referencje i etapy | `Tools/validate_quests.py` |
