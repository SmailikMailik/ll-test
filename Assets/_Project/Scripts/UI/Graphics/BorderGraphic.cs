using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Graphics
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("LL/UI/Graphics/Border")]
    internal sealed class BorderGraphic : MaskableGraphic
    {
        [SerializeField, Min(0f)] private float _thickness = 4f;
        [SerializeField] private BorderAlignment _alignment;

        internal enum BorderAlignment : byte
        {
            Inside = 0,
            Center = 1,
            Outside = 2
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            var rect = GetPixelAdjustedRect();

            if (rect.width <= 0f || rect.height <= 0f)
                return;

            var thickness = Mathf.Min(
                Mathf.Max(0f, _thickness),
                GetMaximumThickness(rect));

            if (thickness <= 0f)
                return;

            var outerOffset = _alignment switch
            {
                BorderAlignment.Center => thickness * 0.5f,
                BorderAlignment.Outside => thickness,
                _ => 0f
            };
            var innerOffset = _alignment switch
            {
                BorderAlignment.Inside => thickness,
                BorderAlignment.Center => thickness * 0.5f,
                _ => 0f
            };
            var outerRect = Expand(rect, outerOffset);
            var innerRect = Expand(rect, -innerOffset);

            AddVertex(vertexHelper, rect, outerRect.xMin, outerRect.yMin);
            AddVertex(vertexHelper, rect, outerRect.xMin, outerRect.yMax);
            AddVertex(vertexHelper, rect, outerRect.xMax, outerRect.yMax);
            AddVertex(vertexHelper, rect, outerRect.xMax, outerRect.yMin);
            AddVertex(vertexHelper, rect, innerRect.xMin, innerRect.yMin);
            AddVertex(vertexHelper, rect, innerRect.xMin, innerRect.yMax);
            AddVertex(vertexHelper, rect, innerRect.xMax, innerRect.yMax);
            AddVertex(vertexHelper, rect, innerRect.xMax, innerRect.yMin);

            AddQuad(vertexHelper, 0, 1, 5, 4);
            AddQuad(vertexHelper, 5, 1, 2, 6);
            AddQuad(vertexHelper, 7, 6, 2, 3);
            AddQuad(vertexHelper, 0, 4, 7, 3);
        }

        private float GetMaximumThickness(Rect rect)
        {
            var minimumSize = Mathf.Min(rect.width, rect.height);

            return _alignment switch
            {
                BorderAlignment.Inside => minimumSize * 0.5f,
                BorderAlignment.Center => minimumSize,
                _ => float.MaxValue
            };
        }

        private static Rect Expand(Rect rect, float amount)
        {
            return Rect.MinMaxRect(
                rect.xMin - amount,
                rect.yMin - amount,
                rect.xMax + amount,
                rect.yMax + amount);
        }

        private void AddVertex(VertexHelper vertexHelper, Rect rect, float x, float y)
        {
            var uv = new Vector2(
                Mathf.InverseLerp(rect.xMin, rect.xMax, x),
                Mathf.InverseLerp(rect.yMin, rect.yMax, y));

            vertexHelper.AddVert(new Vector2(x, y), color, uv);
        }

        private static void AddQuad(
            VertexHelper vertexHelper,
            int bottomLeft,
            int topLeft,
            int topRight,
            int bottomRight)
        {
            vertexHelper.AddTriangle(bottomLeft, topLeft, topRight);
            vertexHelper.AddTriangle(bottomLeft, topRight, bottomRight);
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
            _thickness = Mathf.Max(0f, _thickness);
            SetVerticesDirty();
        }
#endif
    }
}