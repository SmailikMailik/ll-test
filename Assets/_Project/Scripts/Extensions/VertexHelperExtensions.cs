using UnityEngine;
using UnityEngine.UI;

namespace LL.Extensions
{
    internal static class VertexHelperExtensions
    {
        internal static void AddNormalizedVertex(
            this VertexHelper vertexHelper,
            Rect rect,
            Vector2 position,
            Color color)
        {
            var uv = new Vector2(
                Mathf.InverseLerp(rect.xMin, rect.xMax, position.x),
                Mathf.InverseLerp(rect.yMin, rect.yMax, position.y));

            vertexHelper.AddVert(position, color, uv);
        }

        internal static void AddQuad(
            this VertexHelper vertexHelper,
            int bottomLeft,
            int topLeft,
            int topRight,
            int bottomRight)
        {
            vertexHelper.AddTriangle(bottomLeft, topLeft, topRight);
            vertexHelper.AddTriangle(bottomLeft, topRight, bottomRight);
        }

        internal static void AddRectangle(this VertexHelper vertexHelper, Rect rect, Color color)
        {
            var startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddNormalizedVertex(rect, new Vector2(rect.xMin, rect.yMin), color);
            vertexHelper.AddNormalizedVertex(rect, new Vector2(rect.xMin, rect.yMax), color);
            vertexHelper.AddNormalizedVertex(rect, new Vector2(rect.xMax, rect.yMax), color);
            vertexHelper.AddNormalizedVertex(rect, new Vector2(rect.xMax, rect.yMin), color);
            vertexHelper.AddQuad(
                startIndex,
                startIndex + 1,
                startIndex + 2,
                startIndex + 3);
        }
    }
}