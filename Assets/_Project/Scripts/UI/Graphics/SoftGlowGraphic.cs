using LL.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Graphics
{
    internal sealed class SoftGlowGraphic : MaskableGraphic
    {
        [SerializeField] private Vector2 _spread = new(12f, 8f);

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();

            var rect = GetPixelAdjustedRect();
            var xPositions = new[]
            {
                rect.xMin - _spread.x,
                rect.xMin,
                rect.xMax,
                rect.xMax + _spread.x
            };
            var yPositions = new[]
            {
                rect.yMin - _spread.y,
                rect.yMin,
                rect.yMax,
                rect.yMax + _spread.y
            };
            var alphaFactors = new[] { 0f, 1f, 1f, 0f };

            for (var y = 0; y < yPositions.Length; y++)
            {
                for (var x = 0; x < xPositions.Length; x++)
                {
                    var vertexColor = color;
                    vertexColor.a *= alphaFactors[x] * alphaFactors[y];

                    vertexHelper.AddVert(
                        new Vector3(xPositions[x], yPositions[y]),
                        vertexColor,
                        Vector2.zero);
                }
            }

            for (var y = 0; y < yPositions.Length - 1; y++)
            {
                for (var x = 0; x < xPositions.Length - 1; x++)
                {
                    var bottomLeft = y * xPositions.Length + x;
                    var bottomRight = bottomLeft + 1;
                    var topLeft = bottomLeft + xPositions.Length;
                    var topRight = topLeft + 1;

                    vertexHelper.AddQuad(bottomLeft, topLeft, topRight, bottomRight);
                }
            }
        }
    }
}