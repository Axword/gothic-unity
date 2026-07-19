# GAME_DESIGN.md — Filary i zakres gry Żelazna Droga

## 1. Filary gry (Game Pillars)

### 1.1 Surowy, ciężki klimat
- Brudna, przygaszona paleta barw
- Mocne kontrasty świetlne (kierunkowe światło)
- Low/mid-poly styl z czytelnymi sylwetkami
- Atmosfera zagrożenia i nieufności
- Czarny humor w dialogach — "poezja pisana przez skazańca"

### 1.2 Świat ręcznie projektowany
- Brak proceduralnej generacji terenu
- Każda lokacja ma historię i znaczenie
- Zwarte, ale gęste obszary do eksploracji
- Nagrody za dokładną eksplorację (skrytki, skarby)
- Część obszarów początkowo niebezpieczna

### 1.3 Wybór frakcji z konsekwencjami
- Dwie frakcje o wyraźnych filozofiach
- Łańcuchy questów kandydackich
- Trwały wybór wpływający na zakończenie
- Różne nagrody, nauczyciele, ceny w obozach

### 1.4 Rozwój przez nauczycieli
- Brak automatycznego level-upu umiejętności
- Trening u wyspecjalizowanych NPC
- Ograniczeni nauczyciele (limity, wymagania, koszty)
- Sens fabularny treningów

### 1.5 Żywy świat
- Dzień/noc z zmieniającym się oświetleniem
- Harmonogramy NPC (praca, odpoczynek, sen)
- Reakcje na przestępstwa
- Żywa fauna i flora

## 2. Pętla rozgrywki (Core Loop)

```
PRZYBYCIE → ORIENTACJA → EWALUACJA → SZKOLEŃ/WALK → FRAGMENT FABUŁY → WYBÓR FRAKCJI → EPILOG
     ↓            ↓            ↓            ↓              ↓
  Poznanie    Zrozumienie  Próba sił    Rozwój       Decyzja
  świata      konfliktu     w walce      postaci
```

### Faza 1: Przybycie
- Gracz budzi się w nieznanym miejscu
- Pierwszy kontakt z localnymi
- Zrozumienie podstawowych mechanik
-Tutorial walki i interakcji

### Faza 2: Orientacja
- Eksploracja okolicy
- Poznanie obu frakcji
- Zrozumienie konfliktu
- Odkrycie zagrożenia

### Faza 3: Ewaluacja
- Wykonanie questów kandydackich
- Nauka u nauczycieli
- Zdobycie reputacji
- Przygotowanie do wyboru

### Faza 4: Wybór
- Decyzja o przystąpieniu do frakcji
- Blokowanie drugiej ścieżki
- Finalny quest frakcji
- Epilog zależny od wyboru

## 3. Zakres minimalny (Vertical Slice → Pełna gra)

### Vertical Slice (MVP)
- 1 mała lokacja (osada + okolice)
- 5 nazwanych NPC
- 1 potwór
- 1 broń (miecz)
- 1 czar
- 1 skrzynia z zamkiem
- 1 rozgałęzione zadanie
- Pełny core loop

### Pełna gra (v1.0)
- ~8 lokacji (2 osady, trakt, las, bagno, góry, plaża, locus)
- ~65 nazwanych NPC
- ~6 stworów
- ~30 broni (20 mieczy + 10 łuków)
- ~4 zbroje
- ~20 przedmiotów leczniczych/roślin
- ~2 czary
- ~20 zadań głównych + ~10 pobocznych
- Pełny ekwipunek, dialogi, UI

## 4. Balans i progresja

### Brak level scalingu
- Świat jest statyczny
- Gracz rośnie, nie przeciwnicy
- Niektóre obszary celowo niebezpieczne na początku
- Recompensa za powrót z lepszym sprzętem

### Progresja broni
1. **Początkowa (0-3 poziom):** Stare, złamane miecze, kije
2. **Podstawowa (4-6 poziom):** Żelazne miecze, proste łuki
3. **Zaawansowana (7-10 poziom):** Stalowe miecze, dobre łuki
4. **Unikalna (questowa):** Broń konkretnych postaci

### Progresja umiejętności
- **0-33%:** Podstawy (np. 1 combo, 1 poziom zamka)
- **34-66%:** Rozszerzenie (więcej combo, 2 poziom)
- **67-100%:** Mastery (pełne combosy, 3 poziom, specjalne)

## 5. Czas gry

- **Vertical slice:** ~30-60 minut
- **Pełna gra:** ~8-15 godzin (speedrun) / ~20-30 godzin (explo + side)

## 6. Metriki sukcesu

- Gracz może ukończyć grę dowolną ścieżką
- Żaden required content nie jest zablokowany przez bugi
- Wszystkie dialogi mają opcję wyjścia
- Save/load działa poprawnie
- UI jest responsywne i czytelne
