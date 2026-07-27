using LL.UI.Graphics;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.Graphics
{
    [CustomEditor(typeof(BorderGraphic))]
    [CanEditMultipleObjects]
    internal sealed class BorderGraphicEditor : OdinEditor
    {
        private static readonly GUIContent _materialLabel = new("Material");
        private static readonly GUIContent _colorLabel = new("Color");
        private static readonly GUIContent _maskableLabel = new("Maskable");
        private static readonly GUIContent _raycastTargetLabel = new("Raycast Target");
        private static readonly GUIContent _raycastPaddingLabel = new("Raycast Padding");
        private static readonly GUIContent _thicknessLabel = new("Thickness");
        private static readonly GUIContent _alignmentLabel = new("Alignment");

        private SerializedProperty _material;
        private SerializedProperty _color;
        private SerializedProperty _maskable;
        private SerializedProperty _raycastTarget;
        private SerializedProperty _raycastPadding;
        private SerializedProperty _thickness;
        private SerializedProperty _alignment;

        public override void OnInspectorGUI()
        {
            FindProperties();
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawRendering();
            EditorGUILayout.Space(4f);
            DrawBorder();

            var changed = EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                foreach (var inspectedTarget in targets)
                    ((BorderGraphic)inspectedTarget).SetVerticesDirty();
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

        private void DrawBorder()
        {
            SirenixEditorGUI.BeginBox("Border");
            EditorGUILayout.PropertyField(_alignment, _alignmentLabel);
            EditorGUILayout.PropertyField(_thickness, _thicknessLabel);
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
            _thickness = serializedObject.FindProperty("_thickness");
            _alignment = serializedObject.FindProperty("_alignment");
        }
    }
}