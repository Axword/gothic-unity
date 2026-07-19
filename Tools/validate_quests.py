#!/usr/bin/env python3
"""Strict offline validation for quest data and the quest documentation contract."""
import json, pathlib, sys

ROOT = pathlib.Path(__file__).parents[1]
files = sorted((ROOT / 'Assets/StreamingAssets/Data/Json/quests').glob('*.json'))
errors = []
quests = {}
for path in files:
    doc = json.loads(path.read_text(encoding='utf-8'))
    if not isinstance(doc.get('quests'), list): errors.append(f'{path}: missing quests array'); continue
    for q in doc['quests']:
        qid = q.get('id', '<missing>')
        if qid in quests: errors.append(f'{qid}: duplicate quest id')
        quests[qid] = q
        required = ('id','titleKey','descriptionKey','type','requirements','stages','rewards','failConditions')
        for key in required:
            if key not in q: errors.append(f'{qid}: missing {key}')
        for i, stage in enumerate(q.get('stages', []), 1):
            if stage.get('id') != i: errors.append(f'{qid}: stage ids must be sequential (expected {i})')
            if not stage.get('objectives'): errors.append(f'{qid}: stage {i} has no objectives')
            for objective in stage.get('objectives', []):
                if objective.get('type') not in {'Talk','Kill','Collect','Deliver','Explore','Escort','Persuade','Steal','Interact','Craft'}:
                    errors.append(f'{qid}: invalid objective type {objective.get("type")}')
                if not objective.get('target'): errors.append(f'{qid}: objective has no target')
                if objective.get('count', 1) < 1: errors.append(f'{qid}: objective count must be positive')
        nxt = q.get('nextQuest')
        if nxt and nxt not in quests: # checked again below after all files
            pass
for qid, q in quests.items():
    for dep in q.get('requirements', {}).get('quests', []):
        if dep not in quests: errors.append(f'{qid}: unknown prerequisite {dep}')
    if q.get('nextQuest') and q['nextQuest'] not in quests: errors.append(f'{qid}: unknown nextQuest {q["nextQuest"]}')
    faction = qid.split('_')[1] if '_' in qid else ''
    expected = {'Q_M':'Main','Q_F_OLD':'FactionOld','Q_F_NEW':'FactionNew','Q_S':'Side'}
    prefix = '_'.join(qid.split('_')[:3]) if qid.startswith('Q_F_') else '_'.join(qid.split('_')[:2])
    if prefix in expected and q.get('type') != expected[prefix]: errors.append(f'{qid}: type {q.get("type")} != {expected[prefix]}')
if errors:
    print('\n'.join('ERROR: '+e for e in errors)); sys.exit(1)
print(f'PASS: {len(quests)} quests, {sum(len(q["stages"]) for q in quests.values())} stages, {sum(len(s["objectives"]) for q in quests.values() for s in q["stages"])} objectives')
