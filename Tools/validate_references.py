import json,glob,re,pathlib,sys
root=pathlib.Path(__file__).parents[1]/'Assets/StreamingAssets/Data/Json'; records=set(); docs=[]
for p in root.glob('**/*.json'):
 d=json.loads(p.read_text()); docs.append((p,d))
 for v in d.values():
  if isinstance(v,list): records.update(x.get('id') for x in v if isinstance(x,dict) and x.get('id'))
missing=[]
for p,d in docs:
 for key in ('itemId','npcId','monsterId','locationId','questId','startQuest','nextQuest','parentQuest'):
  for x in re.findall(r'"'+key+r'"\s*:\s*"([^"]+)"',json.dumps(d)):
   if x not in records and x != 'gold': missing.append(f'{p.name}: {key}={x}')
if missing:
 print('\n'.join('FAIL '+x for x in sorted(set(missing))));sys.exit(1)
print(f'PASS: {len(records)} stable records, all typed references resolve')
