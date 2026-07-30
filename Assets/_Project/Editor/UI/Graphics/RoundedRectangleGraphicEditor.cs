using LL.UI.Graphics;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.Graphics
{
    [CustomEditor(typeof(RoundedRectangleGraphic))]
    [CanEditMultipleObjects]
    internal sealed class RoundedRectangleGraphicEditor : MaskableGraphicEditorBase
    {
        private static readonly GUIContent _cornerRadiusLabel = new("Corner Radius");
        private static readonly GUIContent _cornerSegmentsLabel = new("Corner Segments");

        private SerializedProperty _cornerRadius;
        private SerializedProperty _cornerSegments;

        protected override void DrawGraphicProperties()
        {
            SirenixEditorGUI.BeginBox("Shape");
            EditorGUILayout.PropertyField(_cornerRadius, _cornerRadiusLabel);
            EditorGUILayout.PropertyField(_cornerSegments, _cornerSegmentsLabel);
            SirenixEditorGUI.EndBox();
        }

        protected override void FindGraphicProperties()
        {
            _cornerRadius = serializedObject.FindProperty("_cornerRadius");
            _cornerSegments = serializedObject.FindProperty("_cornerSegments");
        }
    }
}