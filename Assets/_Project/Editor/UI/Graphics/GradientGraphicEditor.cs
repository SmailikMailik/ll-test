using LL.UI.Graphics;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.Graphics
{
    [CustomEditor(typeof(GradientGraphic))]
    [CanEditMultipleObjects]
    internal sealed class GradientGraphicEditor : MaskableGraphicEditorBase
    {
        private const int LinearTypeIndex = 0;

        private static readonly GUIContent _tintLabel = new("Tint");
        private static readonly GUIContent _gradientLabel = new("Gradient");
        private static readonly GUIContent _typeLabel = new("Type");
        private static readonly GUIContent _angleLabel = new("Angle");
        private static readonly GUIContent _resolutionLabel = new("Resolution");

        private SerializedProperty _gradient;
        private SerializedProperty _type;
        private SerializedProperty _angle;
        private SerializedProperty _resolution;

        protected override GUIContent ColorLabel => _tintLabel;

        protected override void DrawGraphicProperties()
        {
            SirenixEditorGUI.BeginBox("Gradient");
            EditorGUILayout.PropertyField(_gradient, _gradientLabel);
            EditorGUILayout.PropertyField(_type, _typeLabel);

            if (IsLinear())
            {
                EditorGUILayout.PropertyField(_angle, _angleLabel);
                DrawDirectionButtons();
            }

            EditorGUILayout.PropertyField(_resolution, _resolutionLabel);

            if (_resolution.hasMultipleDifferentValues is false)
            {
                var resolution = Mathf.Clamp(_resolution.intValue, 1, 32);
                DrawMeshStatistics(resolution);
            }

            SirenixEditorGUI.EndBox();
        }

        private static void DrawMeshStatistics(int resolution)
        {
            var rowLength = resolution + 1;
            var vertexCount = rowLength * rowLength;
            var triangleCount = resolution * resolution * 2;

            EditorGUILayout.LabelField
            (
                new GUIContent("Generated Mesh", $"{rowLength} × {rowLength} grid"),
                new GUIContent($"{vertexCount:N0} vertices  ·  {triangleCount:N0} triangles")
            );
        }

        private void DrawDirectionButtons()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(EditorGUIUtility.labelWidth);

                DrawAngleButton("→", 0f);
                DrawAngleButton("↑", 90f);
                DrawAngleButton("←", 180f);
                DrawAngleButton("↓", 270f);
            }
        }

        private void DrawAngleButton(string label, float angle)
        {
            if (GUILayout.Button(label, EditorStyles.miniButton))
                _angle.floatValue = angle;
        }

        private bool IsLinear()
        {
            return _type.hasMultipleDifferentValues ||
                   _type.enumValueIndex == LinearTypeIndex;
        }

        protected override void FindGraphicProperties()
        {
            _gradient = serializedObject.FindProperty("_gradient");
            _type = serializedObject.FindProperty("_type");
            _angle = serializedObject.FindProperty("_angle");
            _resolution = serializedObject.FindProperty("_resolution");
        }
    }
}