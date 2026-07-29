using LL.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Graphics
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("LL/UI/Graphics/Rounded Rectangle")]
    internal sealed class RoundedRectangleGraphic : MaskableGraphic
    {
        [Min(0f)]
        [SerializeField] private float _cornerRadius = 16f;
        [Range(MinimumCornerSegments, MaximumCornerSegments)]
        [SerializeField] private int _cornerSegments = 6;

        private const int MinimumCornerSegments = 1;
        private const int MaximumCornerSegments = 16;

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            var rect = GetPixelAdjustedRect();

            if (rect.width <= 0f || rect.height <= 0f)
                return;

            var radius = Mathf.Min(
                Mathf.Max(0f, _cornerRadius),
                Mathf.Min(rect.width, rect.height) * 0.5f);

            if (radius <= 0f)
            {
                vertexHelper.AddRectangle(rect, color);
                return;
            }

            AddRoundedRectangle(
                vertexHelper,
                rect,
                radius,
                Mathf.Clamp(
                    _cornerSegments,
                    MinimumCornerSegments,
                    MaximumCornerSegments));
        }

        private void AddRoundedRectangle(
            VertexHelper vertexHelper,
            Rect rect,
            float radius,
            int cornerSegments)
        {
            vertexHelper.AddNormalizedVertex(rect, rect.center, color);

            AddCorner(
                vertexHelper,
                rect,
                new Vector2(rect.xMin + radius, rect.yMin + radius),
                180f,
                radius,
                cornerSegments);
            AddCorner(
                vertexHelper,
                rect,
                new Vector2(rect.xMax - radius, rect.yMin + radius),
                270f,
                radius,
                cornerSegments);
            AddCorner(
                vertexHelper,
                rect,
                new Vector2(rect.xMax - radius, rect.yMax - radius),
                0f,
                radius,
                cornerSegments);
            AddCorner(
                vertexHelper,
                rect,
                new Vector2(rect.xMin + radius, rect.yMax - radius),
                90f,
                radius,
                cornerSegments);

            var perimeterVertices = vertexHelper.currentVertCount - 1;

            for (var index = 0; index < perimeterVertices; index++)
            {
                var current = index + 1;
                var next = index + 1 < perimeterVertices
                    ? current + 1
                    : 1;

                vertexHelper.AddTriangle(0, current, next);
            }
        }

        private void AddCorner(
            VertexHelper vertexHelper,
            Rect rect,
            Vector2 center,
            float startAngle,
            float radius,
            int segments)
        {
            const float cornerAngle = 90f;

            for (var segment = 0; segment <= segments; segment++)
            {
                var angle = startAngle + cornerAngle * segment / segments;
                var radians = angle * Mathf.Deg2Rad;
                var position = center + new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)) * radius;

                vertexHelper.AddNormalizedVertex(rect, position, color);
            }
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            raycastTarget = false;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            _cornerRadius = Mathf.Max(0f, _cornerRadius);
            _cornerSegments = Mathf.Clamp(
                _cornerSegments,
                MinimumCornerSegments,
                MaximumCornerSegments);
            SetVerticesDirty();
        }
#endif
    }
}