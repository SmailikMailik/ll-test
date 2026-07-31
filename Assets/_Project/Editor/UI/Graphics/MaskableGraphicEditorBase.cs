using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LLEditor.UI.Graphics
{
    internal abstract class MaskableGraphicEditorBase : OdinEditor
    {
        private static readonly GUIContent _materialLabel = new("Material");
        private static readonly GUIContent _colorLabel = new("Color");
        private static readonly GUIContent _maskableLabel = new("Maskable");
        private static readonly GUIContent _raycastTargetLabel = new("Raycast Target");
        private static readonly GUIContent _raycastPaddingLabel = new("Raycast Padding");

        private SerializedProperty _material;
        private SerializedProperty _color;
        private SerializedProperty _maskable;
        private SerializedProperty _raycastTarget;
        private SerializedProperty _raycastPadding;

        protected virtual GUIContent ColorLabel => _colorLabel;

        public sealed override void OnInspectorGUI()
        {
            FindProperties();
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawRendering();
            EditorGUILayout.Space(4f);
            DrawGraphicProperties();

            var changed = EditorGUI.EndChangeCheck();
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                foreach (var inspectedTarget in targets)
                    ((Graphic)inspectedTarget).SetVerticesDirty();
            }
        }

        protected abstract void DrawGraphicProperties();

        protected abstract void FindGraphicProperties();

        private void DrawRendering()
        {
            SirenixEditorGUI.BeginBox("Rendering");
            EditorGUILayout.PropertyField(_material, _materialLabel);
            EditorGUILayout.PropertyField(_color, ColorLabel);
            EditorGUILayout.PropertyField(_maskable, _maskableLabel);
            EditorGUILayout.PropertyField(_raycastTarget, _raycastTargetLabel);

            if (_raycastTarget.boolValue || _raycastTarget.hasMultipleDifferentValues)
                EditorGUILayout.PropertyField(_raycastPadding, _raycastPaddingLabel);

            SirenixEditorGUI.EndBox();
        }

        private void FindProperties()
        {
            if (_material is not null)
                return;

            _material = serializedObject.FindProperty("m_Material");
            _color = serializedObject.FindProperty("m_Color");
            _maskable = serializedObject.FindProperty("m_Maskable");
            _raycastTarget = serializedObject.FindProperty("m_RaycastTarget");
            _raycastPadding = serializedObject.FindProperty("m_RaycastPadding");
            FindGraphicProperties();
        }
    }
}