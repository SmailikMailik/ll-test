using LL.UI.Effects;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.Effects
{
    [CustomEditor(typeof(RoundedRectangleGraphic))]
    [CanEditMultipleObjects]
    internal sealed class RoundedRectangleGraphicEditor : OdinEditor
    {
        private static readonly GUIContent _materialLabel = new("Material");
        private static readonly GUIContent _colorLabel = new("Color");
        private static readonly GUIContent _maskableLabel = new("Maskable");
        private static readonly GUIContent _raycastTargetLabel = new("Raycast Target");
        private static readonly GUIContent _raycastPaddingLabel = new("Raycast Padding");
        private static readonly GUIContent _cornerRadiusLabel = new("Corner Radius");
        private static readonly GUIContent _cornerSegmentsLabel = new("Corner Segments");

        private SerializedProperty _material;
        private SerializedProperty _color;
        private SerializedProperty _maskable;
        private SerializedProperty _raycastTarget;
        private SerializedProperty _raycastPadding;
        private SerializedProperty _cornerRadius;
        private SerializedProperty _cornerSegments;

        public override void OnInspectorGUI()
        {
            FindProperties();
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawRendering();
            EditorGUILayout.Space(4f);
            DrawShape();

            var changed = EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                foreach (var inspectedTarget in targets)
                    ((RoundedRectangleGraphic)inspectedTarget).SetVerticesDirty();
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

        private void DrawShape()
        {
            SirenixEditorGUI.BeginBox("Shape");
            EditorGUILayout.PropertyField(_cornerRadius, _cornerRadiusLabel);
            EditorGUILayout.PropertyField(_cornerSegments, _cornerSegmentsLabel);
            SirenixEditorGUI.EndBox();
        }

        private void FindProperties()
        {
            if (_material != null)
                return;

            _material = serializedObject.FindProperty("m_Material");
            _color = serializedObject.FindProperty("m_Color");
            _maskable = serializedObject.FindProperty("m_Maskable");
            _raycastTarget = serializedObject.FindProperty("m_RaycastTarget");
            _raycastPadding = serializedObject.FindProperty("m_RaycastPadding");
            _cornerRadius = serializedObject.FindProperty("_cornerRadius");
            _cornerSegments = serializedObject.FindProperty("_cornerSegments");
        }
    }
}