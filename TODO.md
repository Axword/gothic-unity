# TODO.md — Lista priorytetów (aktualizacja 2026-07-20)

## P0 — Critical (dla działającego vertical slice)
- [x] Player movement + camera + input
- [x] Stats + leveling + XP + HP
- [x] Inventory + equip + gold
- [x] Basic melee combat + hit detection
- [x] QuestManager + objectives + rewards
- [x] DialogueManager + choices
- [x] Interaction system + NPC + Enemy
- [x] Chest + basic lockpick minigame
- [x] Save / Load (JSON)
- [x] Day/Night + TimeManager
- [x] Editor tool for quick scene setup
- [x] HUD (HP, Mana, Gold)
- [ ] Pełne UI (Inventory panel, Journal, Character sheet) - stub
- [ ] Pełna zawartość JSON (65 NPC, 20 swords, więcej questów, dialogów)

## P1 — High
- [ ] NavMesh + pełne rutyny NPC (harmonogramy)
- [ ] Pełny AI (patrol, alert, group)
- [ ] Lockpick minigame z UI (aktualnie keyboard stub)
- [ ] Trenerzy + umiejętność tree
- [ ] 2 czary + mana casting
- [ ] Więcej lokacji + rośliny + skrzynie w świecie
- [ ] Pełne dialogi wszystkich ważnych NPC (polski)
- [ ] Zbroje wizualne + modele (stub)

## P2
- [ ] Animacje + VFX + audio
- [ ] Main Menu + Pause + Options
- [ ] Pełny zapis stanu świata (otwarte skrzynie, zabici, flagi)
- [ ] Testy automatyczne JSON + quest flow
- [ ] Balance + więcej questów (5 old + 5 new + 10 side)

## Blokery
- Brak assetów 3D (modele, animacje) — używamy kapsuł/sfer
- Brak pełnego UI Toolkit lub rozbudowanego uGUI
- Brak NavMesh w SampleScene

## Co zrobić teraz (dla użytkownika)
1. Otwórz projekt w Unity 6
2. Otwórz SampleScene
3. Menu górne → **Żelazna Droga → Setup Vertical Slice Scene**
4. Play
5. Poruszaj się, rozmawiaj z NPC (E), walcz, otwieraj skrzynie

## Stan po tej iteracji
Vertical Slice jest **grywalny** w podstawowym zakresie (ruch, walka, questy, dialogi, interakcje, save).
