using System;
using System.Collections.Generic;
using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Progression
{
    /// <summary>
    /// Simple reputation system. Affects prices and some dialogues.
    /// </summary>
    public class ReputationSystem : BaseMonoBehaviour
    {
        private Dictionary<string, int> _reputation = new Dictionary<string, int>();

        public static ReputationSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _reputation["OldOrder"] = 0;
            _reputation["NewOrder"] = 0;
            _reputation["Neutral"] = 0;

            ComponentLocator.Register<IReputationSystem>(new ReputationSystemInterface(this));
        }

        public void ChangeReputation(string faction, int delta)
        {
            if (!_reputation.ContainsKey(faction)) _reputation[faction] = 0;
            _reputation[faction] += delta;
            Debug.Log($"[Reputation] {faction} {(_reputation[faction] > 0 ? "+" : "")}{delta} → {_reputation[faction]}");
        }

        public int GetReputation(string faction)
        {
            return _reputation.ContainsKey(faction) ? _reputation[faction] : 0;
        }

        public float GetPriceModifier(string faction)
        {
            int rep = GetReputation(faction);
            if (rep > 20) return 0.8f;
            if (rep > 5) return 0.9f;
            if (rep < -20) return 1.4f;
            if (rep < -5) return 1.2f;
            return 1.0f;
        }
    }

    public interface IReputationSystem
    {
        void ChangeReputation(string faction, int delta);
        int GetReputation(string faction);
        float GetPriceModifier(string faction);
    }

    public class ReputationSystemInterface : IReputationSystem
    {
        private readonly ReputationSystem _rep;
        public ReputationSystemInterface(ReputationSystem r) { _rep = r; }
        public void ChangeReputation(string f, int d) => _rep.ChangeReputation(f, d);
        public int GetReputation(string f) => _rep.GetReputation(f);
        public float GetPriceModifier(string f) => _rep.GetPriceModifier(f);
    }
}
