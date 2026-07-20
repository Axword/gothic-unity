using System;
using UnityEngine;

namespace ZelaznaDroga.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireComponentsAttribute : Attribute
    {
        public Type[] ComponentTypes { get; private set; }
        public RequireComponentsAttribute(params Type[] componentTypes) { ComponentTypes = componentTypes; }
    }
}
