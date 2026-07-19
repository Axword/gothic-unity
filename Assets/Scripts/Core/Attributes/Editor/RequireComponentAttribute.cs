using System;
using UnityEngine;

namespace ZelaznaDroga.Core.Attributes
{
    /// <summary>
    /// Attribute to mark fields that require a specific component.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireComponentsAttribute : Attribute
    {
        public Type[] ComponentTypes { get; private set; }

        public RequireComponentsAttribute(params Type[] componentTypes)
        {
            ComponentTypes = componentTypes;
        }
    }

    /// <summary>
    /// Attribute to mark a class as requiring specific layers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireLayerAttribute : Attribute
    {
        public string[] LayerNames { get; private set; }

        public RequireLayerAttribute(params string[] layerNames)
        {
            LayerNames = layerNames;
        }
    }
}
