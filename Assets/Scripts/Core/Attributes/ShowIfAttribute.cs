using System;
using UnityEngine;

namespace ZelaznaDroga.Core.Attributes
{
    /// <summary>
    /// Base class for all MonoBehaviours in the project.
    /// Provides common functionality and debugging.
    /// </summary>
    public abstract class BaseMonoBehaviour : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] protected bool _isDebugMode = false;
        
        protected virtual void Awake()
        {
            if (_isDebugMode)
            {
                Debug.Log($"[{GetType().Name}] Awake on {gameObject.name}");
            }
        }

        protected virtual void Start()
        {
            if (_isDebugMode)
            {
                Debug.Log($"[{GetType().Name}] Start on {gameObject.name}");
            }
        }

        protected virtual void OnDestroy()
        {
            if (_isDebugMode)
            {
                Debug.Log($"[{GetType().Name}] Destroy on {gameObject.name}");
            }
        }

        /// <summary>
        /// Safe debug log that only outputs in debug mode.
        /// </summary>
        protected void DebugLog(object message)
        {
            if (_isDebugMode)
            {
                Debug.Log($"[{GetType().Name}] {message}");
            }
        }

        /// <summary>
        /// Safe warning log that only outputs in debug mode.
        /// </summary>
        protected void DebugWarning(object message)
        {
            if (_isDebugMode)
            {
                Debug.LogWarning($"[{GetType().Name}] {message}");
            }
        }

        /// <summary>
        /// Safe error log that only outputs in debug mode.
        /// </summary>
        protected void DebugError(object message)
        {
            if (_isDebugMode)
            {
                Debug.LogError($"[{GetType().Name}] {message}");
            }
        }
    }
}

namespace ZelaznaDroga.Core.Attributes
{
    // Attribute for conditional visibility in inspector
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionField { get; private set; }
        public object CompareValue { get; private set; }

        public ShowIfAttribute(string conditionField, object compareValue = null)
        {
            ConditionField = conditionField;
            CompareValue = compareValue;
        }
    }

    // Attribute for read-only fields in inspector
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyAttribute : PropertyAttribute { }

    // Attribute for required references
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : PropertyAttribute { }
}
