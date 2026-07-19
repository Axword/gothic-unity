# TEST_RESULTS.md

Data: 2026-07-19

| Test | Wynik | Dowód |
|---|---|---|
| Parsowanie JSON questów | PASS | `python3 Tools/validate_quests.py` |
| Quest IDs, zależności, etapy, cele | PASS | `PASS: 19 quests, 59 stages, 59 objectives` |
| `git diff --check` | PASS | wykonane po zmianach |
| Unity compile | BLOCKED | brak executable `unity` |
| Unity EditMode/PlayMode | BLOCKED | brak Unity Editor i test assemblies |
| Windows build | BLOCKED | brak Unity CLI |
| Smoke: ruch/dialog/quest/walka | BLOCKED | kod istnieje, brak uruchomienia silnika |
| Save/Load integration | NOT_TESTED | brak harnessu runtime |
| UI/UX visual inspection | BLOCKED | brak uruchomienia sceny |

| Minimalne liczby zawartości | PASS (statycznie) | `python3 Tools/validate_content.py`: swords 20, bows 10, armor 6, plants 10, potions 6, NPC 65, monsters 6, side quests 10 |
| Formuły XP/HP/obrażeń | PASS | `python3 Tools/test_game_rules.py` |
| Przykładowy save i stabilne ID | PASS | `savegame.example.json`, `test_game_rules.py` |
| Referencje, dialogi, rutyny i loot | PASS statycznie | `python3 Tools/validate_references.py`, `python3 Tools/audit_static.py` |

Nie użyto statusu PASS dla testów zależnych od Unity.
