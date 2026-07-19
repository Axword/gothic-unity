# ART_BIBLE.md — Styl wizualny i wytyczne artystyczne

## 1. Styl ogólny

### 1.1 Kierunek artystyczny
**"Low-poly brutalism meets medieval grime"**

- Geometryczne, uproszczone formy (low-poly z subtelnymi detalami)
- Ciężkie, masywne sylwetki
- Brudne tekstury i efekty zużycia
- Kontrastowe oświetlenie kierunkowe
- Minimalistyczne, ale czytelne detale

### 1.2 Inspiracje wizualne
- Gothic 1/2 — surowość, ciężkość
- Dark Souls — czytelność sylwetek, animacje
- Mordhau — realizm w ruchach
- Skyrim (oryginalny) — paleta kolorów

### 1.3 Unikanie
- Over-design i clutter
- Zbyt kolorowe/brzydkie palety
- Clippy/miękkie cienie
- Nadmierny bloom

---

## 2. paleta kolorów

### 2.1 Paleta bazowa

| Nazwa | Hex | Zastosowanie |
|-------|-----|--------------|
| **Ciemny las** | `#1A1F16` | Główne cienie, noce |
| **Ziemia** | `#3D2E1F` | Gleba, drewno, skóra |
| **Kamień** | `#4A4A4A` | Mury, skały |
| **Stal** | `#6B7B8C` | Broń, metal |
| **Rdza** | `#8B4513` | Akcenty metalowe |
| **Krew** | `#8B0000` | Akcenty, niebezpieczeństwo |
| **Las** | `#2D4A2D` | Liście, trawy |
| **Bagno** | `#4A5D3A` | Mokradła, porosty |
| **Niebieski lód** | `#4A6B8C` | Woda, lód, magia |
| **Ogień** | `#CC5500` | Ogień, magia ognia |
| **Kość** | `#E8DCC8` | Kości, kościany loot |
| **Śnieg** | `#D8D8D8` | Skały górskie, akcenty |
| **Złoto** | `#C9A227` | Nagrody, monety |

### 2.2 Paleta frakcyjna

**Gildia Żelaznej Ręki:**
- Dominanta: `#4A4A4A` (stal)
- Akcent: `#8B0000` (karmazyn)
- Drewno: `#5C3D2E` (ciemny dąb)

**Obóz Wolnych:**
- Dominanta: `#3D2E1F` (ziemia)
- Akcent: `#2D4A2D` (las)
- Skóry: `#8B7355` (brąz)

### 2.3 Kolory UI
- Background: `#1A1A1A`
- Panel: `#2A2A2A` z border `#3A3A3A`
- Text primary: `#E8E8E8`
- Text secondary: `#A0A0A0`
- Text warning: `#CC8800`
- Text danger: `#CC3333`
- Text success: `#33AA33`
- Highlight: `#4A6B8C`

---

## 3. Skala i proporcje

### 3.1 Proporcje postaci ludzkich

| Element | Wartość |
|---------|---------|
| Wzrost przeciętny (mężczyzna) | 1.8 m |
| Wzrost przeciętny (kobieta) | 1.65 m |
| Głowa do ciała | 1:7.5 |
| Szerokość ramion (mężczyzna) | 0.5 m |
| Szerokość ramion (kobieta) | 0.45 m |
| Długość kroku | 0.75 m |

### 3.2 Skala world objects

| Obiekt | Skala |
|--------|-------|
| Drzewa iglaste | 8-15 m wysokość |
| Drzewa liściaste | 6-12 m wysokość |
| Skały duże | 3-8 m |
| Skały małe | 0.5-2 m |
| Krzewy | 1-2 m |
| Trawa | 0.3-0.8 m |
| Kamienie (dekoracyjne) | 0.1-0.5 m |
| Domy (1-piętrowe) | 4-5 m wysokość |
| Domy (2-piętrowe) | 7-8 m wysokość |
| Mury | 3-4 m wysokość |

### 3.3 Skala przedmiotów

| Przedmiot | Wymiary |
|-----------|---------|
| Miecz jednoręczny | 1.0-1.2 m długość |
| Miecz dwuręczny | 1.5-2.0 m długość |
| Łuk krótki | 1.2 m wysokość |
| Łuk długi | 1.8 m wysokość |
| Tarcza | 0.6-0.8 m średnica |
| Mikstura (buteleczka) | 0.15 m wysokość |
| Złoto (moneta) | 0.02 m średnica |

---

## 4. Sylwetki i czytelność

### 4.1 Sylwety frakcyjne

**Gildia Żelaznej Ręki:**
- Proste linie, kąty 90°
- Ciężkie, symetryczne formy
- Wyraźne oznaczenia: czerwony herb na tarczy
- Hełmy z osłoną twarzy
- Płaszcze z herbem

**Obóz Wolnych:**
- Organicze, nierówne linie
- Luźne, asymetryczne ubrania
- Skóry zwierzęce jako elementy
- Brody, blizny, tatuaże
- Różnorodność w sylwetkach

### 4.2 Sylwety potworów

| Potwór | Sylweta | Cecha rozpoznawcza |
|--------|---------|-------------------|
| Wilk | Smukła, niska | Długi ogon, ostre uszy |
| Niedźwiedź | Masywna, kwadratowa | Garb, krótki pysk |
| Troll | Wysoka, garbata | Długie ręce, kły |
| Błotna Bestia | Płaska, rozłożysta | Macki, świecące oczy |
| Bandyta | Humanoidalna, zgarbiona | Broń na plecach |
| Demon (Czarne Źródło) | Nieregularna, zmienna | Świecące runy |

### 4.3 Kolory bezpieczeństwa (environmental storytelling)
- Zielone rośliny = bezpieczne / jadalne
- Czerwone jagody = trujące
- Niebieski płyn = mana / magia
- Złote przedmioty = wartościowe
- Czarne plamy = niebezpieczne

---

## 5. Oświetlenie

### 5.1 Kierunki światła

**Dzień:**
- Główne: Directional light, kolor `#FFF5E0`, intensity 1.2, kąt 45°
- Wypełnienie: Ambient, kolor `#4A6080`, intensity 0.3
- Słońce na południe-zachód

**Noc:**
- Główne: Moonlight (Directional), kolor `#4A6080`, intensity 0.2
- Wypełnienie: Ambient, kolor `#1A2030`, intensity 0.1
- Źródła punktowe: Ogniska, pochodnie

**Wnętrza:**
- Directional mild bounce
- Point lights dla źródeł
- Candle lights (ciepły żółty)

### 5.2 Paleta oświetlenia

| Pora dnia | Ambient | Directional | Fog |
|-----------|---------|-------------|-----|
| Dzień | `#6080A0` | `#FFF5E0` | `#C0C0C0` |
| Zmierzch | `#805040` | `#FF8040` | `#604040` |
| Noc | `#203050` | `#4060A0` | `#101520` |
| Bagno (dzień) | `#405030` | `#A0B080` | `#304020` |

### 5.3 Effect lighting
- Ogień: `#FF6600`, pulsujące
- Magia ognia: `#FF4400` + `#FFFF00`
- Magia lodu: `#40A0FF` + `#FFFFFF`
- Czarne Źródło: `#4000FF` + `#8000FF`

---

## 6. Materiały i tekstury

### 6.1 Style materiałów

**PBR base z stylizowanymi modyfikacjami:**
- Brak normal maps dla stylizacji
- Stylizowane roughnes maps (less detailed)
- Ciężkie stylizowane AO

### 6.2 Typy powierzchni

| Materiał | Albedo | Roughness | Metallic |
|----------|--------|-----------|----------|
| Drewno stare | `#3D2E1F` | 0.8 | 0 |
| Drewno nowe | `#5C3D2E` | 0.7 | 0 |
| Kamień ciemny | `#3A3A3A` | 0.9 | 0 |
| Kamień jasny | `#6A6A6A` | 0.85 | 0 |
| Metal nierdzewny | `#6B7B8C` | 0.4 | 0.9 |
| Metal rdzawy | `#8B4513` | 0.7 | 0.3 |
| Skóra | `#4A3020` | 0.7 | 0 |
| Tkanina | `#3A3A4A` | 0.9 | 0 |

### 6.3 Stylizacja broni

| Typ broni | Styl | Detale |
|-----------|------|--------|
| Stara/złamana | Nierówna, rdza, ślady użytku | Brak ozdób |
| Żelazna | Prosta, solidna | Minimalne ozdoby |
| Stalowa | Gładka, błyszcząca | Inkrustacje |
| Unikalna | Wyraźny design | Runy, ozdoby frakcyjne |

---

## 7. Budżety assetów

### 7.1 Statystyki polygonów

| Typ | Triangle budget | Polecany |
|-----|-----------------|----------|
| Gracz (with weapons) | 15,000 | 10,000 |
| NPC | 8,000 | 5,000 |
| Potwór mały (wilk) | 3,000 | 2,000 |
| Potwór średni (niedźwiedź) | 8,000 | 5,000 |
| Potwór duży (troll) | 20,000 | 15,000 |
| Broń (miecz) | 500 | 300 |
| Zbroja | 3,000 | 2,000 |
| Drzewa (LOD0) | 2,000 | 1,000 |
| Skały (LOD0) | 1,000 | 500 |
| Budynki (LOD0) | 10,000 | 5,000 |

### 7.2 Statystyki tekstur

| Typ | Rozmiar | Format |
|-----|---------|--------|
| Characters | 1024x1024 | DXT5 |
| Weapons | 512x512 | DXT5 |
| Armor | 1024x1024 | DXT5 |
| Environment tiles | 512x512 | DXT1 |
| UI icons | 128x128 | DXT5 |
| UI panels | 256x256 | DXT5 |

### 7.3 LOD distances

| Typ | LOD 0 | LOD 1 | LOD 2 | LOD 3 |
|-----|-------|-------|-------|-------|
| Postacie | 0-10m | 10-30m | 30-60m | 60m+ |
| Potwory | 0-15m | 15-40m | 40-80m | 80m+ |
| Drzewa | 0-20m | 20-50m | 50m+ | - |
| Skały | 0-30m | 30-80m | 80m+ | - |
| Budynki | 0-50m | 50-150m | 150m+ | - |

---

## 8. Animacje

### 8.1 Style animacji

- **Realistyczne timing:** Ruchy ciężkie, momentum
- **Anticipation:** Krótkie "cofnięcie" przed akcją
- **Follow-through:** Naturalne zakończenia ruchów
- **Brak clipping:** Sprawdzanie kolizji animacji

### 8.2 Animation priorities

| Priorytet | Typy animacji |
|-----------|---------------|
| P0 (critical) | Idle, Walk, Run, Jump, Fall |
| P1 (combat) | AttackLight, AttackHeavy, Block, Hit, Death |
| P2 (interaction) | Talk, Work, Sit, Sleep, Eat |
| P3 (styling) | Idle variants, Reaction animations |

### 8.3 Combo animations (miecz)

| Combo | Nazwa | Trigger | Frames |
|-------|-------|---------|--------|
| 1 | Slash | LMB | 0-20 |
| 2 | Slash-Slash | LMB-LMB | 20-40 |
| 3 | Slash-Slash-Overhead | LMB-LMB-LMB | 40-70 |
| Heavy | Overhead | RMB | 0-40 |
| Heavy-2 | Overhead-Stab | RMB-RMB | 40-80 |

### 8.4 Blend trees

- **Locomotion:** Idle ↔ Walk ↔ Run (speed parameter)
- **Combat Idle:** Idle-Sword ↔ Block-Ready
- **Upper Body:** (weapon layer) Idle ↔ Attack

---

## 9. VFX

### 9.1 Style VFX

- **Prosty styl:** Minimalne particle count
- **Kolory z palety:** Nie RGB out-of-range
- **Silne kontrasty:** Bright on dark

### 9.2 VFX inventory

| Effect | Particle count | Opacity | Duration |
|--------|---------------|---------|----------|
| Sword hit | 20-30 | 0.8 | 0.2s |
| Blood splatter | 15-25 | 0.9 | 0.3s |
| Fire (torch) | 50-100 | 0.7 | loop |
| Magic projectile | 30-50 | 0.9 | 0.5s |
| Magic impact | 40-60 | 1.0 | 0.3s |
| Heal effect | 60-80 | 0.6 | 1.0s |

### 9.3 Shader effects (URP)

- Stylized toon shading
- Rim lighting (subtle)
- Simple outline (optional)
- Color grading via post-process

---

## 10. Audio style

### 10.1 Dźwięki otoczenia

- **Ambient:** Eerie forest, wind, water
- **Weather:** Rain, thunder, fog
- **Time-based:** Birds (day), crickets (night)

### 10.2 Dźwięki walki

| Action | Style | Reference |
|--------|-------|-----------|
| Sword swing | Whoosh, heavy | Metal whoosh |
| Sword hit | Impact, flesh/metal | Thud + metal |
| Arrow shoot | Twang + whistle | Bow release |
| Arrow hit | Thud | Wood impact |
| Magic cast | Charge + release | Energy build |
| Magic hit | Burst + crackle | Elemental burst |

### 10.3 Muzyka

- **Menu:** Dark, atmospheric, minimal
- **Overworld:** Subtle ambient, tribal drums (obóz)
- **Combat:** Intense, drums, strings
- **Boss:** Epic, choir, low brass
- **Epilog:** Reflective, resolve

---

## 11. IK i rig

### 11.1 Humanoid rig requirements

- Full humanoid avatar
- IK hands (for two-handed weapons)
- IK feet (for uneven terrain)
- Look-at for head
- Procedural breathing (subtle)

### 11.2 Animation layering

```
Base Layer:     Locomotion (Full Body)
Combat Layer:   Upper Body Override (weapon)
Interaction:    Additive (object interaction)
```

---

## 12. Post-processing

### 12.1 Volume settings

| Setting | Day | Night | Combat |
|---------|-----|-------|--------|
| Exposure | 1.0 | 0.6 | 1.1 |
| Contrast | 1.1 | 1.2 | 1.15 |
| Saturation | 0.9 | 0.7 | 1.0 |
| Vignette | 0.3 | 0.4 | 0.5 |
| Bloom | 0.1 | 0.2 | 0.3 |

### 12.2 Color grading

- Slight teal shadows
- Warm highlights
- Desaturated midtones
- Orange & teal LUT for cinematic feel
