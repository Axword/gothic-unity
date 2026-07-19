import json, pathlib, sys
R=pathlib.Path(__file__).parents[1]/'Assets/StreamingAssets/Data/Json'
def count(rel,key): return len(json.loads((R/rel).read_text())[key])
checks=[('items/items_weapons_swords.json','items',20),('items/items_weapons_bows.json','items',10),('items/items_armors.json','items',4),('items/items_plants.json','items',10),('items/items_potions.json','items',6),('npcs/npcs.json','npcs',65),('monsters/monsters.json','monsters',6),('quests/quests_side.json','quests',10)]
failed=[]
for f,k,target in checks:
 n=count(f,k); print(f'{f}: {n}/{target} '+('PASS' if n>=target else 'FAIL'))
 if n<target: failed.append(f)
if failed: sys.exit(1)
print('PASS: content minimums')
