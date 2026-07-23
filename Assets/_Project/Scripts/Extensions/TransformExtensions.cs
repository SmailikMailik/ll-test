using UnityEngine;

namespace LL.Extensions
{
    internal static class TransformExtensions
    {
        internal static void DestroyAllChildren(this Transform transform)
        {
            if (transform == null || transform.childCount == 0)
                return;

            for (var i = 0; i < transform.childCount; i++)
                Object.Destroy(transform.GetChild(i).gameObject);
        }

        internal static void DestroyAllChildren<T>(this Transform transform) where T : Component
        {
            if (transform == null || transform.childCount == 0)
                return;

            foreach (var component in transform.GetComponentsInChildren<T>())
                Object.Destroy(component.gameObject);
        }

        internal static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}