# RELEASE_READINESS.md

## Ocena: BLOCKED — STATIC AUDIT ONLY

Projekt ma działający szkic architektury, dane JSON i proceduralny vertical slice, ale nie spełnia kryteriów wydania.

## Najważniejsze ustalenia

### Questy

Statycznie poprawne:

- 19 questów;
- 59 etapów;
- 59 celów;
- 4 główne;
- 6 Old Order;
- 6 New Order;
- 3 poboczne.

Braki względem wymagań:

- wymagane jest minimum 10 questów pobocznych, obecnie są 3;
- nie potwierdzono runtime’owej integracji danych z właściwym `QuestManager`;
- demonstracyjny `VerticalSliceQuest` działa jako osobny uproszczony system.

### Zawartość

| Kategoria | Stan | Wymagane |
|---|---:|---:|
| Miecze | 8 | 20 |
| Łuki | 5 | 10 |
| Zbroje | 6 | 4 |
| Rośliny | 10 | 10 |
| Mikstury | 6 | 6 |
| NPC | 9 | około 65 |
| Potwory | 6 | minimum 6 |
| Questy poboczne | 3 | minimum 10 |

### Najważniejsze problemy

- **P0:** brak możliwości wykonania buildu i uruchomienia Unity;
- **P1:** brak potwierdzonej integracji vertical slice z produkcyjnymi systemami;
- **P1:** braki wymaganej zawartości;
- **P1:** brak pełnej ścieżki wyboru frakcji i epilogu;
- **P2:** proceduralny greybox zamiast finalnych modeli i animacji;
- **P2:** brak pełnego testu zapisu/wczytania;
- **P2:** brak pełnej integracji NPC, rutyn, AI i systemu przestępstw.


### Blockery

1. Brak Unity Editor/CLI w środowisku — nie potwierdzono kompilacji, uruchomienia ani buildu.
2. Runtime vertical slice nie jest podłączony do produkcyjnych `QuestManager`, `DialogueManager` i `CombatSystem`; używa klas demonstracyjnych.
3. Zawartość poniżej wymagań: 8/20 mieczy, 5/10 łuków, 9/~65 NPC, 3/10 questów pobocznych.
4. Brak scen, prefabów, finalnych assetów i pełnej integracji UI.

### Co jest potwierdzone statycznie

- JSON questów przechodzi `Tools/validate_quests.py`.
- Repozytorium jest na właściwym branchu i bez lokalnych zmian po audycie.
- Schemat assembly definitions oraz pliki projektu Unity istnieją.

Status nie może być wyższy niż `BLOCKED — STATIC AUDIT ONLY` do czasu uruchomienia Unity i usunięcia P0/P1.
