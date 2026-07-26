using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Effects
{
    internal sealed class GradientGraphic : MaskableGraphic
    {
        [SerializeField] private Gradient _gradient = CreateDefaultGradient();
        [SerializeField] private GradientType _type;
        [SerializeField, Range(0f, 360f)] private float _angle;
        [SerializeField, Range(1, 32)] private int _resolution = 8;

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            var rect = GetPixelAdjustedRect();
            var resolution = Mathf.Clamp(_resolution, 1, 32);
            var direction = GetDirection();
            var minimumProjection = GetMinimumProjection(rect, direction);
            var maximumProjection = GetMaximumProjection(rect, direction);
            var center = rect.center;
            var maximumRadius = Vector2.Distance(center, new Vector2(rect.xMax, rect.yMax));

            for (var y = 0; y <= resolution; y++)
            {
                for (var x = 0; x <= resolution; x++)
                {
                    var normalizedPosition = new Vector2(
                        (float)x / resolution,
                        (float)y / resolution);
                    var position = new Vector2(
                        Mathf.Lerp(rect.xMin, rect.xMax, normalizedPosition.x),
                        Mathf.Lerp(rect.yMin, rect.yMax, normalizedPosition.y));
                    var gradientPosition = GetGradientPosition(
                        position,
                        direction,
                        minimumProjection,
                        maximumProjection,
                        center,
                        maximumRadius);
                    var vertexColor = color * _gradient.Evaluate(gradientPosition);

                    vertexHelper.AddVert(position, vertexColor, normalizedPosition);
                }
            }

            var rowLength = resolution + 1;

            for (var y = 0; y < resolution; y++)
            {
                for (var x = 0; x < resolution; x++)
                {
                    var bottomLeft = y * rowLength + x;
                    var bottomRight = bottomLeft + 1;
                    var topLeft = bottomLeft + rowLength;
                    var topRight = topLeft + 1;

                    vertexHelper.AddTriangle(bottomLeft, topLeft, topRight);
                    vertexHelper.AddTriangle(bottomLeft, topRight, bottomRight);
                }
            }
        }

        protected override void OnEnable()
        {
            _gradient ??= CreateDefaultGradient();
            base.OnEnable();
        }

        private float GetGradientPosition(
            Vector2 position,
            Vector2 direction,
            float minimumProjection,
            float maximumProjection,
            Vector2 center,
            float maximumRadius)
        {
            return _type switch
            {
                GradientType.Radial when maximumRadius > 0f =>
                    Vector2.Distance(center, position) / maximumRadius,
                _ => Mathf.InverseLerp(
                    minimumProjection,
                    maximumProjection,
                    Vector2.Dot(position, direction))
            };
        }

        private Vector2 GetDirection()
        {
            var radians = _angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        private static float GetMinimumProjection(Rect rect, Vector2 direction)
        {
            var leftMinimum = Mathf.Min(
                Vector2.Dot(new Vector2(rect.xMin, rect.yMin), direction),
                Vector2.Dot(new Vector2(rect.xMin, rect.yMax), direction));
            var rightMinimum = Mathf.Min(
                Vector2.Dot(new Vector2(rect.xMax, rect.yMin), direction),
                Vector2.Dot(new Vector2(rect.xMax, rect.yMax), direction));

            return Mathf.Min(leftMinimum, rightMinimum);
        }

        private static float GetMaximumProjection(Rect rect, Vector2 direction)
        {
            var leftMaximum = Mathf.Max(
                Vector2.Dot(new Vector2(rect.xMin, rect.yMin), direction),
                Vector2.Dot(new Vector2(rect.xMin, rect.yMax), direction));
            var rightMaximum = Mathf.Max(
                Vector2.Dot(new Vector2(rect.xMax, rect.yMin), direction),
                Vector2.Dot(new Vector2(rect.xMax, rect.yMax), direction));

            return Mathf.Max(leftMaximum, rightMaximum);
        }

        private static Gradient CreateDefaultGradient()
        {
            var gradient = new Gradient();
            gradient.SetKeys(
                new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(new Color(1f, 0.92f, 0.65f), 1f)
                },
                new[]
                {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 1f)
                });

            return gradient;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            _gradient ??= CreateDefaultGradient();
            _resolution = Mathf.Clamp(_resolution, 1, 32);
            SetVerticesDirty();
        }
#endif
    }
}