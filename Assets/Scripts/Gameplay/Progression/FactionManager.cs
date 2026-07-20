using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Progression
{
    /// <summary>
    /// Simple faction choice system. Once chosen, the other path is blocked.
    /// </summary>
    public class FactionManager : BaseMonoBehaviour
    {
        public enum Faction { None, OldOrder, NewOrder }

        private Faction _chosen = Faction.None;

        public Faction ChosenFaction => _chosen;

        public static FactionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ComponentLocator.Register<IFactionManager>(new FactionManagerInterface(this));
        }

        public bool ChooseFaction(string faction)
        {
            if (_chosen != Faction.None)
            {
                Debug.Log("[Faction] Already chosen!");
                return false;
            }

            if (faction.ToLower().Contains("old") || faction == "OldOrder")
            {
                _chosen = Faction.OldOrder;
                Debug.Log("[Faction] Joined OLD ORDER (Gildia)");
                // Start epilogue
                var qm = ComponentLocator.Get<IQuestManager>();
                qm?.StartQuest("Q_M_FINAL_OLD");
                return true;
            }
            else if (faction.ToLower().Contains("new") || faction == "NewOrder")
            {
                _chosen = Faction.NewOrder;
                Debug.Log("[Faction] Joined NEW ORDER (Wolni)");
                var qm = ComponentLocator.Get<IQuestManager>();
                qm?.StartQuest("Q_M_FINAL_NEW");
                return true;
            }
            return false;
        }

        public bool IsFactionChosen() => _chosen != Faction.None;
        public bool IsOldOrder() => _chosen == Faction.OldOrder;
        public bool IsNewOrder() => _chosen == Faction.NewOrder;
    }

    public interface IFactionManager
    {
        bool ChooseFaction(string faction);
        bool IsFactionChosen();
    }

    public class FactionManagerInterface : IFactionManager
    {
        private readonly FactionManager _fm;
        public FactionManagerInterface(FactionManager fm) { _fm = fm; }
        public bool ChooseFaction(string f) => _fm.ChooseFaction(f);
        public bool IsFactionChosen() => _fm.IsFactionChosen();
    }
}
