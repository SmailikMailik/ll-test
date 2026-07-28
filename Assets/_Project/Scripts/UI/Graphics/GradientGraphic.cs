using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Graphics
{
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("LL/UI/Graphics/Gradient")]
    internal sealed class GradientGraphic : MaskableGraphic
    {
        [SerializeField] private Gradient _gradient = CreateDefaultGradient();
        [SerializeField] private GradientType _type;
        [SerializeField, Range(0f, 360f)] private float _angle;
        [SerializeField, Range(1, 32)] private int _resolution = 16;

        internal enum GradientType : byte
        {
            Linear = 0,
            Radial = 1
        }

        private Texture2D _alphaTexture;
        private bool _isAlphaTextureDirty = true;

        public override Texture mainTexture
        {
            get
            {
                RebuildAlphaTextureIfNeeded();
                return _alphaTexture;
            }
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            RebuildAlphaTextureIfNeeded();

            var rect = GetPixelAdjustedRect();
            var resolution = Mathf.Clamp(_resolution, 1, 32);
            var direction = GetDirection();
            var minimumProjection = GetMinimumProjection(rect, direction);
            var maximumProjection = GetMaximumProjection(rect, direction);
            var center = rect.center;
            var radius = rect.size * 0.5f;

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
                        radius);
                    var gradientColor = _gradient.Evaluate(gradientPosition);
                    var vertexColor = new Color(
                        color.r * gradientColor.r,
                        color.g * gradientColor.g,
                        color.b * gradientColor.b,
                        color.a);

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
            _isAlphaTextureDirty = true;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            ReleaseAlphaTexture();
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            _isAlphaTextureDirty = true;
            base.OnRectTransformDimensionsChange();
        }

        private void RebuildAlphaTextureIfNeeded()
        {
            if (_isAlphaTextureDirty is false && _alphaTexture != null)
                return;

            _gradient ??= CreateDefaultGradient();

            var resolution = Mathf.Clamp(_resolution, 1, 32);
            var textureSize = resolution + 1;

            if (_alphaTexture == null ||
                _alphaTexture.width != textureSize ||
                _alphaTexture.height != textureSize)
            {
                ReleaseAlphaTexture();
                _alphaTexture = new Texture2D(
                    textureSize,
                    textureSize,
                    TextureFormat.RGBA32,
                    false)
                {
                    name = $"{nameof(GradientGraphic)} Alpha",
                    hideFlags = HideFlags.HideAndDontSave,
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp
                };
            }

            var rect = GetPixelAdjustedRect();
            var direction = GetDirection();
            var minimumProjection = GetMinimumProjection(rect, direction);
            var maximumProjection = GetMaximumProjection(rect, direction);
            var center = rect.center;
            var radius = rect.size * 0.5f;
            var pixels = new Color32[textureSize * textureSize];

            for (var y = 0; y < textureSize; y++)
            {
                for (var x = 0; x < textureSize; x++)
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
                        radius);
                    var alpha = _gradient.Evaluate(gradientPosition).a;

                    pixels[y * textureSize + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            _alphaTexture.SetPixels32(pixels);
            _alphaTexture.Apply(false, false);
            _isAlphaTextureDirty = false;
        }

        private void ReleaseAlphaTexture()
        {
            if (_alphaTexture == null)
                return;

            if (Application.isPlaying)
                Destroy(_alphaTexture);
            else
                DestroyImmediate(_alphaTexture);

            _alphaTexture = null;
        }

        private float GetGradientPosition(
            Vector2 position,
            Vector2 direction,
            float minimumProjection,
            float maximumProjection,
            Vector2 center,
            Vector2 radius)
        {
            return _type switch
            {
                GradientType.Radial => GetRadialGradientPosition(position, center, radius),
                _ => Mathf.InverseLerp(minimumProjection, maximumProjection, Vector2.Dot(position, direction))
            };
        }

        private static float GetRadialGradientPosition(Vector2 position, Vector2 center, Vector2 radius)
        {
            if (radius.x <= 0f || radius.y <= 0f)
                return 0f;

            var offset = position - center;
            var normalizedOffset = new Vector2(
                offset.x / radius.x,
                offset.y / radius.y);
            return Mathf.Clamp01(normalizedOffset.magnitude);
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
                    new GradientColorKey(Color.white, 1f)
                },
                new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0f, 1f)
                });

            return gradient;
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
            _gradient ??= CreateDefaultGradient();
            _resolution = Mathf.Clamp(_resolution, 1, 32);
            _isAlphaTextureDirty = true;
            SetVerticesDirty();
            SetMaterialDirty();
        }
#endif
    }
}