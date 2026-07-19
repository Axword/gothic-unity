#!/usr/bin/env python3
"""Deterministic offline regression tests for formulas and save invariants."""
import json, pathlib, tempfile
ROOT=pathlib.Path(__file__).parents[1]; balance=json.loads((ROOT/'Assets/StreamingAssets/Data/Json/balance.json').read_text())
assert balance['xpPerLevel'] > 0 and balance['hpPerLevel'] > 0 and balance['learningPointsPerLevel'] > 0

def level_for_xp(xp): return max(1, xp // balance['xpPerLevel'] + 1)
def max_hp(level, base=100): return base + (level-1)*balance['hpPerLevel']
def melee_damage(base, strength, armor): return max(0, base + strength*balance['damage']['strengthPerPoint'] - armor)
def spell_damage(base, power, resist): return max(0, base + power - resist)
assert level_for_xp(0) == 1 and level_for_xp(100) == 2
assert max_hp(2) == 115 and max_hp(10) > max_hp(1)
assert melee_damage(10, 5, 3) == 12 and melee_damage(1, 0, 99) == 0
assert spell_damage(20, 4, 10) == 14
# Save invariant: quest rewards are represented as stable, unique IDs and schema version.
sample=json.loads((ROOT/'Assets/StreamingAssets/Data/Json/savegame.example.json').read_text())
assert isinstance(sample['saveVersion'], str) and sample['saveVersion']
assert len(sample['completedQuests']) == len(set(sample['completedQuests']))
assert sample['player']['level'] >= 1
print('PASS: XP, HP, damage and save invariants')
