using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZelaznaDroga.Core.Utilities
{
    /// <summary>
    /// Simple event bus for decoupled communication between systems.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _listeners = new Dictionary<Type, List<Delegate>>();
        private static readonly object _lock = new object();

        /// <summary>
        /// Subscribe to an event.
        /// </summary>
        public static void Subscribe<T>(Action<T> callback) where T : struct
        {
            lock (_lock)
            {
                Type eventType = typeof(T);
                if (!_listeners.ContainsKey(eventType))
                {
                    _listeners[eventType] = new List<Delegate>();
                }
                _listeners[eventType].Add(callback);
            }
        }

        /// <summary>
        /// Unsubscribe from an event.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> callback) where T : struct
        {
            lock (_lock)
            {
                Type eventType = typeof(T);
                if (_listeners.TryGetValue(eventType, out List<Delegate> callbacks))
                {
                    callbacks.Remove(callback);
                }
            }
        }

        /// <summary>
        /// Publish an event to all subscribers.
        /// </summary>
        public static void Publish<T>(T eventData) where T : struct
        {
            lock (_lock)
            {
                Type eventType = typeof(T);
                if (_listeners.TryGetValue(eventType, out List<Delegate> callbacks))
                {
                    // Create a copy to avoid issues with modifications during iteration
                    Delegate[] callbacksCopy = callbacks.ToArray();
                    foreach (Delegate callback in callbacksCopy)
                    {
                        try
                        {
                            ((Action<T>)callback)(eventData);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"EventBus: Error in event handler for {eventType.Name}: {ex}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear all listeners.
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _listeners.Clear();
            }
        }
    }

    // Game-specific event types
    #region Game Events

    /// <summary>
    /// Event fired when player's health changes.
    /// </summary>
    public struct PlayerHealthChangedEvent
    {
        public readonly int CurrentHealth;
        public readonly int MaxHealth;
        public readonly int Delta;

        public PlayerHealthChangedEvent(int current, int max, int delta)
        {
            CurrentHealth = current;
            MaxHealth = max;
            Delta = delta;
        }
    }

    /// <summary>
    /// Event fired when player's mana changes.
    /// </summary>
    public struct PlayerManaChangedEvent
    {
        public readonly int CurrentMana;
        public readonly int MaxMana;
        public readonly int Delta;

        public PlayerManaChangedEvent(int current, int max, int delta)
        {
            CurrentMana = current;
            MaxMana = max;
            Delta = delta;
        }
    }

    /// <summary>
    /// Event fired when player levels up.
    /// </summary>
    public struct PlayerLevelUpEvent
    {
        public readonly int NewLevel;
        public readonly int LearningPointsGained;

        public PlayerLevelUpEvent(int newLevel, int pointsGained)
        {
            NewLevel = newLevel;
            LearningPointsGained = pointsGained;
        }
    }

    /// <summary>
    /// Event fired when player dies.
    /// </summary>
    public struct PlayerDiedEvent
    {
        public readonly string KillerId;

        public PlayerDiedEvent(string killerId = null)
        {
            KillerId = killerId;
        }
    }

    /// <summary>
    /// Event fired when a quest starts.
    /// </summary>
    public struct QuestStartedEvent
    {
        public readonly string QuestId;

        public QuestStartedEvent(string questId)
        {
            QuestId = questId;
        }
    }

    /// <summary>
    /// Event fired when a quest stage completes.
    /// </summary>
    public struct QuestStageCompletedEvent
    {
        public readonly string QuestId;
        public readonly int StageIndex;

        public QuestStageCompletedEvent(string questId, int stageIndex)
        {
            QuestId = questId;
            StageIndex = stageIndex;
        }
    }

    /// <summary>
    /// Event fired when a quest completes.
    /// </summary>
    public struct QuestCompletedEvent
    {
        public readonly string QuestId;

        public QuestCompletedEvent(string questId)
        {
            QuestId = questId;
        }
    }

    /// <summary>
    /// Event fired when a quest fails.
    /// </summary>
    public struct QuestFailedEvent
    {
        public readonly string QuestId;
        public readonly string Reason;

        public QuestFailedEvent(string questId, string reason = null)
        {
            QuestId = questId;
            Reason = reason;
        }
    }

    /// <summary>
    /// Event fired when an NPC is killed.
    /// </summary>
    public struct NpcKilledEvent
    {
        public readonly string NpcId;
        public readonly string KillerId;

        public NpcKilledEvent(string npcId, string killerId)
        {
            NpcId = npcId;
            KillerId = killerId;
        }
    }

    /// <summary>
    /// Event fired when item is picked up.
    /// </summary>
    public struct ItemPickupEvent
    {
        public readonly string ItemId;
        public readonly int Count;

        public ItemPickupEvent(string itemId, int count = 1)
        {
            ItemId = itemId;
            Count = count;
        }
    }

    /// <summary>
    /// Event fired when crime is witnessed.
    /// </summary>
    public struct CrimeWitnessedEvent
    {
        public readonly string WitnessId;
        public readonly string CriminalId;
        public readonly CrimeType Crime;

        public CrimeWitnessedEvent(string witnessId, string criminalId, CrimeType crime)
        {
            WitnessId = witnessId;
            CriminalId = criminalId;
            Crime = crime;
        }
    }

    public enum CrimeType
    {
        Theft,
        Trespass,
        Assault,
        Murder
    }

    /// <summary>
    /// Event fired when faction reputation changes.
    /// </summary>
    public struct FactionReputationChangedEvent
    {
        public readonly string FactionId;
        public readonly int NewReputation;
        public readonly int Delta;

        public FactionReputationChangedEvent(string factionId, int newRep, int delta)
        {
            FactionId = factionId;
            NewReputation = newRep;
            Delta = delta;
        }
    }

    /// <summary>
    /// Event fired when player chooses a faction.
    /// </summary>
    public struct FactionChosenEvent
    {
        public readonly string FactionId;

        public FactionChosenEvent(string factionId)
        {
            FactionId = factionId;
        }
    }

    /// <summary>
    /// Event fired when time of day changes.
    /// </summary>
    public struct TimeOfDayChangedEvent
    {
        public readonly int Hour;
        public readonly TimeOfDay TimeOfDay;

        public TimeOfDayChangedEvent(int hour, TimeOfDay time)
        {
            Hour = hour;
            TimeOfDay = time;
        }
    }

    public enum TimeOfDay
    {
        Night,
        Dawn,
        Morning,
        Noon,
        Afternoon,
        Dusk,
        Evening,
        Midnight
    }

    /// <summary>
    /// Event fired when dialogue starts.
    /// </summary>
    public struct DialogueStartedEvent
    {
        public readonly string DialogueId;
        public readonly string NpcId;

        public DialogueStartedEvent(string dialogueId, string npcId)
        {
            DialogueId = dialogueId;
            NpcId = npcId;
        }
    }

    /// <summary>
    /// Event fired when dialogue ends.
    /// </summary>
    public struct DialogueEndedEvent
    {
        public readonly string DialogueId;
        public readonly bool WasCompleted;

        public DialogueEndedEvent(string dialogueId, bool completed)
        {
            DialogueId = dialogueId;
            WasCompleted = completed;
        }
    }

    #endregion
}
