using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZelaznaDroga.Core.Extensions
{
    /// <summary>
    /// Common extension methods for Unity types.
    /// </summary>
    public static class UnityExtensions
    {
        /// <summary>
        /// Attempts to get a component, returning false if not found.
        /// </summary>
        public static bool TryGetComponent<T>(this Component component, out T result) where T : class
        {
            result = component as T;
            return result != null;
        }

        /// <summary>
        /// Safely destroys a GameObject or Component.
        /// </summary>
        public static void SafeDestroy(this UnityEngine.Object obj)
        {
            if (obj == null) return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            else
#endif
            {
                UnityEngine.Object.Destroy(obj);
            }
        }

        /// <summary>
        /// Safely destroys all children of a Transform.
        /// </summary>
        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                transform.GetChild(i).gameObject.SafeDestroy();
            }
        }

        /// <summary>
        /// Gets or adds a component.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out T component))
            {
                return component;
            }
            return gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Sets layer recursively on a GameObject and all its children.
        /// </summary>
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        /// <summary>
        /// Checks if a layer mask contains a specific layer.
        /// </summary>
        public static bool ContainsLayer(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        /// <summary>
        /// Converts a Vector3 to a formatted string.
        /// </summary>
        public static string ToFormattedString(this Vector3 vector, string format = "F2")
        {
            return $"({vector.x.ToString(format)}, {vector.y.ToString(format)}, {vector.z.ToString(format)})";
        }

        /// <summary>
        /// Converts a Quaternion to Euler angles formatted string.
        /// </summary>
        public static string ToEulerString(this Quaternion quaternion, string format = "F1")
        {
            Vector3 euler = quaternion.eulerAngles;
            return $"({euler.x.ToString(format)}, {euler.y.ToString(format)}, {euler.z.ToString(format)})";
        }

        /// <summary>
        /// Returns the inverse of a direction, or the original if zero.
        /// </summary>
        public static Vector3 SafeInverse(this Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.0001f)
                return Vector3.zero;
            return direction.normalized;
        }

        /// <summary>
        /// Gets the angle between two directions, always positive.
        /// </summary>
        public static float AngleBetween(Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from.SafeInverse(), to.SafeInverse());
        }
    }
}
