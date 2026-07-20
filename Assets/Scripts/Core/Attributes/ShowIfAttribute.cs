using System;
using UnityEngine;

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
