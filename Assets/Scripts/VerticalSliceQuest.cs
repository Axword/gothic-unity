using UnityEngine;
using UnityEngine.UI;

namespace ZelaznaDroga
{
    public enum VerticalQuestState { Locked, Active, Completed }

    /// <summary>Small vertical-slice quest flow, independent from production data systems.</summary>
    public sealed class VerticalSliceQuest : MonoBehaviour
    {
        public VerticalQuestState State { get; private set; } = VerticalQuestState.Locked;
        public int ObjectiveProgress { get; private set; }
        private Text questText;

        public void Configure(Text output) { questText = output; Refresh(); }

        public void StartQuest()
        {
            if (State != VerticalQuestState.Locked) return;
            State = VerticalQuestState.Active;
            ObjectiveProgress = 0;
            Refresh();
        }

        public void CompleteObjective()
        {
            if (State != VerticalQuestState.Active) return;
            ObjectiveProgress = 1;
            State = VerticalQuestState.Completed;
            Refresh();
        }

        private void Refresh()
        {
            if (questText == null) return;
            questText.text = State == VerticalQuestState.Locked
                ? "ZADANIE\nBrak aktywnego zadania"
                : State == VerticalQuestState.Active
                    ? "ZADANIE: Ślad przy starym młynie\nCel: Zbadaj miejsce przy młynie (0/1)"
                    : "ZADANIE UKOŃCZONE\nŚlad przy starym młynie odnaleziony";
        }
    }
}
