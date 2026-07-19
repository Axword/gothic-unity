# THIRD_PARTY_ASSETS.md — Obce assety

Wszystkie assety używane w projekcie z odpowiednimi licencjami.

---

## Własności projektu

Wszystkie niżej wymienione assety są tworzone jako placeholdery/proceduralne i stanowią własność projektu:
- Modele 3D (placeholder low-poly)
- Tekstury procedural
- Dźwięki procedural (jeśli użyte)
- Kod (własny)

---

## Do użycia (dozwolone)

### Assety Unity Asset Store (Free)

| Asset | Autor | Licencja | Użycie |
|-------|-------|----------|--------|
| Fantasy Adventure Music (Lite) | Firelight | Unity Asset Store EULA | Muzyka ambient |
| Essential Low Poly Weapons | Goblin/Shaders | Unity Asset Store EULA | Placeholder broni |
| Nature Starter Kit 2 | Unity Technologies | Unity Asset Store EULA | Placeholder środowiska |

**Uwaga:** Asset Store EULA pozwala na użycie w projektach komercyjnych i niekomercyjnych.

---

## Proceduralne zamienniki

Zamiast używać zewnętrznych assetów, projekt używa proceduralnych/zamienników:

### Grafika
- **Modele:** Proste primitive (cube, cylinder) z shaderami stylizowanymi
- **Ikony:** SVG/białe ikony na colored background (generowane)
- **UI:** uGUI z custom shaderami

### Audio
- **SFX:** Brak na v0.9 (cisza)
- **Muzyka:** Brak na v0.9
- **Ambient:** Brak na v0.9

---

## Wymagane attributions (jeśli użyte)

Jeśli projekt użyje poniższych, należy dodać attribution:

### Kenney.nl Assets
- **URL:** https://kenney.nl/assets
- **Licencja:** CC0 1.0 Universal
- **Do użycia:** Tak, attribution wymagany

### OpenGameArt Sounds
- **URL:** https://opengameart.org
- **Licencja:** Various ( sprawdzać indywidualnie)
- **Do użycia:** Tak, z weryfikacją licencji

---

## Nie używamy

Poniższe są WYKREŚLONE z projektu:

- ❌ Gothic/Gothic II assets (ochrona IP)
- ❌ Risen assets (ochrona IP)  
- ❌ Elder Scrolls assets (ochrona IP)
- ❌ Asset store full packs bez weryfikacji licencji

---

## Weryfikacja licencji

Przed dodaniem assetu z zewnętrznego źródła:

1. ✅ Sprawdź licencję
2. ✅ Upewnij się, że pozwala na użycie komercyjne
3. ✅ Zachowaj attribution w kodzie/comentach
4. ✅ Dodaj do tej listy
5. ✅ Pobierz i zachowaj w Assets/ThirdParty/

```
Assets/ThirdParty/
├── [AssetName]/
│   ├── License.txt
│   └── Readme.txt
```

---

## Template dla nowego assetu

```markdown
### [Asset Name]
- **Źródło:** [URL]
- **Autor:** [Author]
- **Licencja:** [License name]
- **Wersja:** [Version]
- **Data dodania:** [YYYY-MM-DD]
- **Użycie:** [Co używamy]
- **Attribution:** [Jeśli wymagane]
```

---

## Status: CZYSTE

Obecnie projekt nie używa żadnych zewnętrznych assetów z wymaganym attribution.
Wszystkie assety są:
- ✅ Własne (projekt)
- ✅ Proceduralne (wygenerowane)
- ✅ Unity Built-in
