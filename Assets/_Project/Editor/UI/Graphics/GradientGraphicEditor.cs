using LL.UI.Graphics;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.Graphics
{
    [CustomEditor(typeof(GradientGraphic))]
    [CanEditMultipleObjects]
    internal sealed class GradientGraphicEditor : OdinEditor
    {
        private static readonly GUIContent _materialLabel = new("Material");
        private static readonly GUIContent _colorLabel = new("Tint");
        private static readonly GUIContent _raycastTargetLabel = new("Raycast Target");
        private static readonly GUIContent _raycastPaddingLabel = new("Raycast Padding");
        private static readonly GUIContent _maskableLabel = new("Maskable");
        private static readonly GUIContent _gradientLabel = new("Gradient");
        private static readonly GUIContent _typeLabel = new("Type");
        private static readonly GUIContent _angleLabel = new("Angle");
        private static readonly GUIContent _resolutionLabel = new("Resolution");

        private SerializedProperty _material;
        private SerializedProperty _color;
        private SerializedProperty _raycastTarget;
        private SerializedProperty _raycastPadding;
        private SerializedProperty _maskable;
        private SerializedProperty _gradient;
        private SerializedProperty _type;
        private SerializedProperty _angle;
        private SerializedProperty _resolution;

        public override void OnInspectorGUI()
        {
            FindProperties();
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawRendering();
            EditorGUILayout.Space(4f);
            DrawGradient();

            var changed = EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                foreach (var inspectedTarget in targets)
                    ((GradientGraphic)inspectedTarget).SetVerticesDirty();
            }
        }

        private void DrawRendering()
        {
            SirenixEditorGUI.BeginBox("Rendering");
            EditorGUILayout.PropertyField(_material, _materialLabel);
            EditorGUILayout.PropertyField(_color, _colorLabel);
            EditorGUILayout.PropertyField(_maskable, _maskableLabel);
            EditorGUILayout.PropertyField(_raycastTarget, _raycastTargetLabel);

            if (_raycastTarget.boolValue || _raycastTarget.hasMultipleDifferentValues)
                EditorGUILayout.PropertyField(_raycastPadding, _raycastPaddingLabel);

            SirenixEditorGUI.EndBox();
        }

        private void DrawGradient()
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
                   _type.enumValueIndex == (int)GradientGraphic.GradientType.Linear;
        }

        private void FindProperties()
        {
            if (_material != null)
                return;

            _material = serializedObject.FindProperty("m_Material");
            _color = serializedObject.FindProperty("m_Color");
            _raycastTarget = serializedObject.FindProperty("m_RaycastTarget");
            _raycastPadding = serializedObject.FindProperty("m_RaycastPadding");
            _maskable = serializedObject.FindProperty("m_Maskable");
            _gradient = serializedObject.FindProperty("_gradient");
            _type = serializedObject.FindProperty("_type");
            _angle = serializedObject.FindProperty("_angle");
            _resolution = serializedObject.FindProperty("_resolution");
        }
    }
}