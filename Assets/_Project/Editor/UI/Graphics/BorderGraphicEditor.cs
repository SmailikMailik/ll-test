using LL.UI.Graphics;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.Graphics
{
    [CustomEditor(typeof(BorderGraphic))]
    [CanEditMultipleObjects]
    internal sealed class BorderGraphicEditor : MaskableGraphicEditorBase
    {
        private static readonly GUIContent _thicknessLabel = new("Thickness");
        private static readonly GUIContent _alignmentLabel = new("Alignment");

        private SerializedProperty _thickness;
        private SerializedProperty _alignment;

        protected override void DrawGraphicProperties()
        {
            SirenixEditorGUI.BeginBox("Border");
            EditorGUILayout.PropertyField(_alignment, _alignmentLabel);
            EditorGUILayout.PropertyField(_thickness, _thicknessLabel);
            SirenixEditorGUI.EndBox();
        }

        protected override void FindGraphicProperties()
        {
            _thickness = serializedObject.FindProperty("_thickness");
            _alignment = serializedObject.FindProperty("_alignment");
        }
    }
}