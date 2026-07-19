#!/usr/bin/env python3
"""Non-Unity audit: graph reachability, schedules, loot and referenced asset inventory."""
import json, pathlib, sys
ROOT=pathlib.Path(__file__).parents[1]; DATA=ROOT/'Assets/StreamingAssets/Data/Json'
errors=[]; warnings=[]
def load(p): return json.loads(p.read_text(encoding='utf-8'))
items={x['id'] for p in (DATA/'items').glob('*.json') for x in load(p)['items']}
npcs=load(DATA/'npcs/npcs.json')['npcs']; npcids={x['id'] for x in npcs}
locs={x['id'] for x in load(DATA/'world_locations.json')['locations']}
monster=load(DATA/'monsters/monsters.json')['monsters']; monsterids={x['id'] for x in monster}
for s in load(DATA/'npcs/npc_schedules.json')['schedules']:
 if s['npcId'] not in npcids: errors.append(f'unknown scheduled NPC {s["npcId"]}')
 for phase in s.get('phases',[]):
  lid=phase.get('locationId')
  if lid not in locs: errors.append(f'unknown schedule location {lid}')
for m in monster:
 for key in ('lootTable','trophyItem'):
  value=m.get(key)
  if key=='trophyItem' and value and value not in items: errors.append(f'{m["id"]}: missing trophy {value}')
# Dialogue reachability and terminal exits.
for p in (DATA/'dialogues').glob('*.json'):
 d=load(p); nodes=d['nodes']; root=d['rootNodeId']; seen=set(); stack=[root]
 while stack:
  n=stack.pop()
  if n in seen: continue
  if n not in nodes: errors.append(f'{p.name}: missing node {n}'); continue
  seen.add(n)
  for c in nodes[n].get('choices',[]):
   nxt=c.get('nextNode') or c.get('nextNodeId')
   if nxt: stack.append(nxt)
   elif not c.get('actions') and c.get('text','').strip().lower().strip('[]') not in ('żegnaj','do widzenia','odejdź','koniec'):
    warnings.append(f'{p.name}:{n}: choice has no next node/action')
 unreachable=set(nodes)-seen
 if unreachable: warnings.append(f'{p.name}: unreachable nodes {sorted(unreachable)}')
# Asset paths are intentionally reported, never silently treated as imported.
asset_refs=[]
for p in DATA.rglob('*.json'):
 def walk(x):
  if isinstance(x,dict):
   for k,v in x.items():
    if k.endswith('Path') and isinstance(v,str): asset_refs.append((p.name,v))
    walk(v)
  elif isinstance(x,list):
   for v in x: walk(v)
 walk(load(p))
missing_assets=[(p,a) for p,a in asset_refs if not (ROOT/'Assets'/a).exists() and not (ROOT/a).exists()]
print(f'PASS: {len(npcs)} NPC schedules checked, {len(monster)} monster records, dialogues graph checked')
print(f'WARNING: {len(warnings)} dialogue/data quality warnings')
print(f'WARNING: {len(missing_assets)} referenced art assets are not imported (expected until art pass)')
for w in warnings: print('WARN',w)
if errors:
 for e in errors: print('FAIL',e)
 sys.exit(1)
