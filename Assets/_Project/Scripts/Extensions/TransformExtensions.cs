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
    }
}