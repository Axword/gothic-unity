# QUESTS_AND_DIALOGUES.md — System zadań i dialogów

## 1. Architektura systemu questów

### 1.1 Typy zadań

| Typ | Prefiks | Opis |
|-----|---------|------|
| Główny | Q_M | Fabularny, wymagany do ukończenia |
| Fração | Q_F_* | Kandydacki dla frakcji |
| Poboczny | Q_S | Opcjonalny, nagrody |
| Ukryty | Q_H | Bez logowania, easter eggs |

### 1.2 Struktura questu

```json
{
  "id": "Q_M_001",
  "title": "Nazwa zadania",
  "description": "Opis w dzienniku",
  "type": "Main|FactionOld|FactionNew|Side",
  "faction": "None|OldOrder|NewOrder",
  "requirements": {
    "level": 1,
    "quests": ["Q_M_000"],
    "flags": ["met_npc_aldona"],
    "items": ["item_old_coin"]
  },
  "stages": [
    {
      "id": 1,
      "description": "Cel etapu",
      "objectives": [
        { "type": "Talk", "target": "npc_aldona", "description": "Porozmawiaj z Mistrzynią" },
        { "type": "Kill", "target": "monster_wolf", "count": 3, "description": "Zabij 3 wilki" }
      ],
      "onComplete": { "addFlags": ["talked_aldona"], "giveItems": [], "startQuest": null },
      "onFail": { "failQuest": true }
    }
  ],
  "rewards": {
    "xp": 500,
    "gold": 100,
    "items": ["item_herb_healing"],
    "flags": ["reputation_old_1"],
    "skillPoints": 1
  },
  "failConditions": [],
  "parentQuest": null,
  "nextQuest": "Q_M_002"
}
```

### 1.3 Typy celów

| Typ | Parametry | Przykład |
|-----|-----------|----------|
| Talk | target | Porozmawiaj z NPC |
| Kill | target, count | Zabij X stworów |
| Collect | item, count | Zbierz X przedmiotów |
| Deliver | item, target | Dostarcz przedmiot do NPC |
| Explore | location | Odkryj lokację |
| Escort | target | Eskortuj NPC |
| Persuade | target, difficulty | Przekonaj NPC |
| Steal | item, target | Ukradnij przedmiot |
| Interact | target | Użyj interaktywnego obiektu |
| Craft | item | Wytwórz przedmiot |

---

## 2. Graf zależności questów

```
                    [Q_M_001 PRZYBYCIE]
                           │
                           ▼
              ┌────────────────────────┐
              │    Q_M_002 ORIENTACJA   │
              │  (poznaj oba obozy)     │
              └───────────┬─────────────┘
                          │
         ┌────────────────┴────────────────┐
         ▼                                 ▼
  ┌──────────────┐                 ┌──────────────┐
  │ Q_F_OLD_* (5) │                 │ Q_F_NEW_* (5)│
  │   GILDIA      │                 │   WOLNI      │
  └───────┬───────┘                 └───────┬──────┘
          │                                 │
          └────────────┬───────────────────┘
                       ▼
              ┌──────────────────┐
              │  Q_M_010 WYBÓR   │
              │ (zablokuj ścieżkę│
              │  drugiej frakcji)│
              └────────┬─────────┘
                       ▼
              ┌──────────────────┐
              │  Q_FINAL_*        │
              │ (final quest      │
              │  wybranej frakcji)│
              └────────┬─────────┘
                       ▼
              ┌──────────────────┐
              │   Q_EPILOG       │
              │ (zakończenie     │
              │  zależne od      │
              │  wyboru)         │
              └──────────────────┘

SIDE QUESTS (rozproszone, niezależne):
Q_S_001 - Q_S_010 (dostępne w dowolnym momencie po spełnieniu warunków)
```

---

## 3. Main Quest Line

### Q_M_001: Przebudzenie w Mrozie
- **Trigger:** Start gry
- **Opis:** Budzisz się w zimnym wąwozie, nie pamiętasz jak tu trafiłeś. Musisz znaleźć drogę do osady.
- **Etapy:**
  1. Rozejrzyj się (Explore trigger)
  2. Znajdź wejście do krainy (Explore "Zejście")
  3. Spotkaj Wędrowca (Talk)
- **Nagroda:** 50 XP, stary nóż (jeśli explore)
- **Uwagi:** Tutorial interakcji

### Q_M_002: Dwie Ścieżki
- **Trigger:** Q_M_001 complete, dotarcie do Traktu
- **Opis:** Widzisz dwie drogi: do Kamiennego Brzegu i Obóz pod Dębami. Musisz poznać oba miejsca.
- **Etapy:**
  1. Udaj się do Kamiennego Brzegu (Explore)
  2. Porozmawiaj z kimś w mieście (Talk)
  3. Udaj się do Obóz pod Dębami (Explore)
  4. Porozmawiaj z kimś w obozie (Talk)
- **Nagroda:** 100 XP
- **Odblokowuje:** Q_F_OLD_001 i Q_F_NEW_001

### Q_M_010: Wybór Drogi
- **Trigger:** 3 questy kandydackie jednej frakcji
- **Opis:** Zasługujesz na przyjęcie do jednej z grup. Ale musisz wybrać.
- **Etapy:**
  1. Podejmij decyzję (choice dialog)
- **Konsekwencje:**
  - Old Order: +reputacja Gildii, -reputacja Wolnych
  - New Order: +reputacja Wolnych, -reputacja Gildii
- **Blokuje:** Drugą ścieżkę kandydacką

---

## 4. Questy Gildii Żelaznej Ręki

### Q_F_OLD_001: Pierwszy Obowiązek
- **Wymagania:** Q_M_002
- **Opis:** Mistrzyni Aldona wysyła cię na polowanie na wilki terroryzujące farmerów.
- **Etapy:**
  1. Idź do farmy Kowalskiego (Explore)
  2. Zabij 3 wilki (Kill 3x wolf)
  3. Przynieś dowód (Collect 3x wolf Pelt)
  4. Raportuj Mistrzyni (Talk)
- **Nagroda:** 150 XP, 50 sztuk złota, Żelazny Miecz

### Q_F_OLD_002: Egzekutor Prawa
- **Wymagania:** Q_F_OLD_001, Level 3
- **Opis:** Strażnik Boruk potrzebuje pomocy w pojmaniu złodzieja.
- **Etapy:**
  1. Porozmawiaj z Borukiem (Talk)
  2. Znajdź złodzieja w lesie (Explore)
  3. [WYBÓR] Pojmaj go żywcem LUB zabij
  4. Zwróć złodzieja Borukowi (Talk)
- **Nagroda:** 200 XP, 80 złota, Dostęp do treningu walki
- **Uwagi:** Różne opcje dialogowe

### Q_F_OLD_003: Towar na Sprzedaż
- **Wymagania:** Q_F_OLD_002
- **Opis:** Rzemieślnik Młynarczyk potrzebuje rudy z kamieniołomów.
- **Etapy:**
  1. Porozmawiaj z Młynarczykiem (Talk)
  2. Zbierz 5 sztuk rudy (Collect)
  3. Dostarcz rudę (Talk)
- **Nagroda:** 180 XP, 60 złota, Zniżka w sklepie Młynarczyka (permanent flag)
- **Bonus:** Jeśli przyniesiesz 10 rud (opcjonalny cel): 50 bonus XP

### Q_F_OLD_004: Nieświęte Relikwie
- **Wymagania:** Q_F_OLD_003, Level 5
- **Opis:** Odkryto stare artefakty w kamieniołomach. Mistrzyni chce je zbadać.
- **Etapy:**
  1. Porozmawiaj z Aldoną (Talk)
  2. Zbadaj wejście do kamieniołomów (Explore)
  3. Znajdź artefakt (Collect)
  4. [NIEBEZPIECZEŃSTWO] Uniknij trolla LUB go zabij
  5. Przynieś artefakt Aldonie (Talk)
- **Nagroda:** 350 XP, 100 złota, Unikalna zbroja Gildii
- **Uwagi:** Troll jest trudny na tym poziomie

### Q_F_OLD_005: Zdrada w Szeregach
- **Wymagania:** Q_F_OLD_004, Level 7
- **Opis:** Doniesiono o sabotażyście w Gildii.
- **Etapy:**
  1. Porozmawiaj z Aldoną (Talk)
  2. Przesłuchaj podejrzanych (Talk x3)
  3. [WYBÓR] Kogo oskarżysz? (3 opcje)
  4. Konfrontacja (Persuade lub Kill)
- **Nagroda:** 400 XP, 150 złota, Strażnik honorowy (tytuł)
- **Uwagi:** Niesłuszne oskarżenie = inny NPC ma pretensje

### Q_F_OLD_FINAL:宣誓 Gildii
- **Wymagania:** Q_F_OLD_005, wszystkie 5 questów
- **Opis:** Czas na ostateczny ślub z Gildią.
- **Etapy:**
  1. Przyjdź na ceremonię (Explore)
  2. Złóż przysięgę (Choice)
  3. Epilog
- **Nagroda:** Tytuł, unikalne itemy, alternatywne zakończenie

---

## 5. Questy Obóz Wolnych

### Q_F_NEW_001: Krew za Krew
- **Wymagania:** Q_M_002
- **Opis:** Wódz Drwal chce dowodu twojej wartości — głowa lidera bandytów.
- **Etapy:**
  1. Porozmawiaj z Drwalem (Talk)
  2. Znajdź obozowisko bandytów (Explore)
  3. Zabij Macznę (Kill boss)
  4. Przynieś głowę (Collect)
- **Nagroda:** 150 XP, 50 złota, Miecz Maczny (używany)
- **Uwagi:** Można spróbować perswazji zamiast walki (trudniejsze)

### Q_F_NEW_002: Zęby i Pazury
- **Wymagania:** Q_F_NEW_001, Level 3
- **Opis:** Lowca Grom chce futer na zimę.
- **Etapy:**
  1. Porozmawiaj z Gromem (Talk)
  2. Zbierz 5 skór wilków (Skin wolves)
  3. Zbierz 2 skóry niedźwiedzia (Skin bear)
  4. Przynieś łup Gromowi (Talk)
- **Nagroda:** 200 XP, 80 złota, Dostęp do treningu łuku
- **Uwagi:** Wymaga nauczyciela skórowania

### Q_F_NEW_003: Handel Nielegalny
- **Wymagania:** Q_F_NEW_002
- **Opis:** Kosa chce przemycić towar przez posterunek Gildii.
- **Etapy:**
  1. Porozmawiaj z Kosą (Talk)
  2. Przemieść towar (Escort/Stealth)
  3. [WYBÓR] Przekup strażnika LUB uniknij LUB walcz
- **Nagroda:** 180 XP, 100 złota, Kosa szanuje cię bardziej
- **Bonus:** Jeśli nikt nie zginie: +50 bonus XP

### Q_F_NEW_004: Stare Sztuczki
- **Wymagania:** Q_F_NEW_003, Level 5
- **Opis:** Szamanek Płomienny widzi w tobie potencjał magiczny.
- **Etapy:**
  1. Porozmawiaj z Płomienną (Talk)
  2. Znajdź jej sekretne miejsce w mokradłach (Explore)
  3. Przetrwaj próbę (interact + survive)
  4. Naucz się podstaw magii (skill gain)
- **Nagroda:** 350 XP, umiejętność Fireball, Respect +1
- **Uwagi:** Wymaga Minimum 5 Zręczności

### Q_F_NEW_005: Zemsta jest Daninem
- **Wymagania:** Q_F_NEW_004, Level 7
- **Opis:** Gildia zaatakowała obóz. Czas na odwet.
- **Etapy:**
  1. Porozmawiaj z Drwalem (Talk)
  2. Wybierz cel [WYBÓR]: Magazyn Gildii LUB Posterunek LUB Dwór
  3. Wykonaj atak (Kill + Steal mix)
  4. Wróć do Drwala (Talk)
- **Nagroda:** 400 XP, 200 złota, +2 Respect w obozie
- **Uwagi:** Wybór ma konsekwencje w epilogu

### Q_F_NEW_FINAL: Ogień Wolności
- **Wymagania:** Q_F_NEW_005, wszystkie 5 questów
- **Opis:** Czas zakończyć panowanie Gildii.
- **Etapy:**
  1. Zbierz armię Wolnych (Talk x3)
  2. Marsz na Kamienny Brzeg (Explore)
  3. Epilog
- **Nagroda:** Tytuł Wolnego, unikalne itemy, alternatywne zakończenie

---

## 6. Questy Poboczne

### Q_S_001: Grzybobranie
- **Trigger:** Dostępny od startu
- **Opis:** Zbierz grzyby dla babci w Kamiennym Brzegu.
- **Etapy:** Zbierz 5 grzybów → Porozmawiaj z babcią
- **Nagroda:** 30 XP, 20 złota, zapas jedzenia

### Q_S_002: Zguba
- **Trigger:** Dostępny po Q_M_002
- **Opis:** Ktoś zgubił medalion w lesie.
- **Etapy:** Znajdź medalion → Zwróć właścicielowi
- **Nagroda:** 50 XP, historia o zmarłym

### Q_S_003: Wodna Bestia
- **Trigger:** Level 4+, po wejściu do mokradeł
- **Opis:** Farmę atakuje stwór z bagna.
- **Etapy:** Zbadaj farmę → Zabij Błotną Bestię → Nagroda
- **Nagroda:** 200 XP, 100 złota, Skóra bestii

### Q_S_004-010: (pełna lista w plikach JSON)

---

## 7. Dialogi — Struktura

### 7.1 Węzeł dialogowy

```json
{
  "nodeId": "dlg_aldona_001",
  "speaker": "npc_aldona",
  "text": "Cóż, nieznajomy. Kim jesteś i czego szukasz w mojej osadzie?",
  "choices": [
    {
      "text": "Jestem podróżnym. Szukam schronienia.",
      "conditions": [],
      "actions": [{ "type": "SetFlag", "value": "said_traveler" }],
      "nextNode": "dlg_aldona_002a"
    },
    {
      "text": "Mam misję od samego króla!",
      "conditions": [{ "type": "HasItem", "item": "item_fake_letter" }],
      "actions": [{ "type": "SetFlag", "value": "claimed_royal" }],
      "nextNode": "dlg_aldona_002b",
      "failNode": "dlg_aldona_002c"
    },
    {
      "text": "To nie twoja sprawa.",
      "conditions": [],
      "actions": [{ "type": "ChangeRelation", "value": -5 }],
      "nextNode": "dlg_aldona_002d"
    },
    {
      "text": "[Odejdź]",
      "conditions": [],
      "actions": [],
      "nextNode": null
    }
  ],
  "onEnter": [],
  "onExit": []
}
```

### 7.2 Warunki dialogowe

| Typ | Opis |
|-----|------|
| HasItem | Gracz ma przedmiot |
| HasFlag | Flaga jest ustawiona |
| QuestActive | Quest jest aktywny |
| QuestComplete | Quest ukończony |
| LevelGte | Poziom gracza >= wartość |
| ReputationGte | Reputacja w frakcji >= wartość |
| SkillGte | Umiejętność >= wartość |
| TimeOfDay | Określona pora dnia |
| Random | Losowy % |

### 7.3 Akcje dialogowe

| Typ | Efekt |
|-----|-------|
| StartQuest | Rozpoczyna quest |
| CompleteQuestStage | Kończy etap questu |
| FailQuest | Fail questu |
| SetFlag | Ustawia flagę |
| ClearFlag | Czyści flagę |
| GiveItem | Daje przedmiot |
| RemoveItem | Zabiera przedmiot |
| ChangeRelation | Zmienia relację |
| ChangeReputation | Zmienia reputację |
| Teleport | Teleportuje gracza |
| PlayAnimation | Odtwarza animację NPC |
| AddShop | Dodaje dostęp do sklepu |
| UnlockTrainer | Odblokowuje nauczyciela |

---

## 8. Przykładowe dialogi (polskie)

### Dlg: Pierwsze spotkanie z Aldoną (Q_M_002)
```
[ALDONA siedzi za biurkiem, nie podnosząc wzroku]
"Tak? Widzę, że nie masz munduru Gildii. Mów szybko, co cię do nas przygnało."

> "Szukam pracy i dachu nad głową."
  [ALDONA kiwa głową]
  "Praca? Mamy jej pod dostatkiem. Ale najpierw musisz udowodnić, że nie jesteś 
   pierwszym lepszym rzezimieszkiem. Strażnik Boruk potrzebuje rąk do polowania. 
   To dobry początek."

> "Przysłał mnie list od Mistrza Żelaznego."
  [ALDONA podnosi wzrok, mierzy cię]
  "List? Pokaż mi go."
  [Jeśli masz list: "Hmmm... Mistrz Żelazny nie wspominał o nikim. Ale niech będzie.
   Mistrzyni Aldona kiwa głową.] 
  [Jeśli nie masz: "Nie mam żadnego listu. Kłamiesz? Czy jesteś głupi?" 
   - reputacja -10]

> "Nic. Przepraszam za扰."
  [ALDona macha ręką]
  "Co za marnowanie mojego czasu."

> "[Odejdź]"
```

### Dlg: Odrzucenie przez Wolnych (Q_M_002)
```
[DRWAL siedzi przy ognisku, sączy piwo]
"Napatrzyłem się na twoją twarz. Co jest? Gildia cię wyrzuciła? A może 
uciekłeś spod cepów?"

> "Chcę dołączyć do Wolnych."
  [DRWAL parska śmiechem]
  "Dołączyć? Ha! Każdy dureń chce 'wolności'. Pokaż mi, że nie jesteś 
   pierwszym lepszym mięczakiem. Za obozem grasuje banda Maczny. Głowa 
   Maczny — i możemy porozmawiać."

> "Gildia mnie nie interesuje."
  [DRWAL uśmiecha się krzywo]
  "To dobry początek. Ale musisz coś zrobić, żebyśmy cię nie zjedli 
   przy kolacji."

> "[Odejdź]"
```

---

## 9. System konsekwencji

### 9.1 Reputacja

| Wartość | Nazwa | Efekt |
|---------|-------|-------|
| -100 do -50 | Wróg | Natychmiastowa walka |
| -49 do -20 | Nieufny | Odmowa dialogu, wyzywanie |
| -19 do 0 | Obojętny | Neutralne traktowanie |
| 1 do 20 | Znany | Przyzwoite traktowanie |
| 21 do 50 | Szanowany | Zniżki, dodatkowe opcje |
| 51 do 100 | Bohater | Unikalne questy, itemy |

### 9.2 Przykłady konsekwencji

| Akcja | Konsekwencja |
|-------|--------------|
| Ukradnij u strażnika | -20 rep Gildia, hunt mode |
| Pomóż farmerowi | +5 rep Gildia, +1 złoto |
| Zabij NPC Wolnych | -30 rep Wolni, revenge quest |
| Skórkuj wszystkie wilki | +2 skill Skinning |
| Zdradź zleceniodawcę | Nowa flaga, -10 rep |

---

## 10. Lokalizacja tekstów

Wszystkie teksty są w osobnych plikach JSON z kluczami:

```
Assets/StreamingAssets/Localization/pl/
├── dialogs.json
├── quests.json
├── items.json
├── npcs.json
├── ui.json
└── world.json
```

Format:
```json
{
  "key": "dlg_aldona_001_text",
  "value": "Cóż, nieznajomy. Kim jesteś i czego szukasz?"
}
```

Dla v1.0 językiem bazowym jest Polski.
