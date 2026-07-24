using LL.UI.Bar.Progress;
using UnityEditor;

namespace LLEditor
{
    [CustomEditor(typeof(ProgressBar))]
    internal sealed class ProgressBarEditor : Editor
    {
        private SerializedProperty _stripType;
        private SerializedProperty _foregroundImage;
        private SerializedProperty _shadowImage;

        private SerializedProperty _valueType;
        private SerializedProperty _valueLabel;
        private SerializedProperty _valueFormat;

        private void OnEnable()
        {
            _stripType = serializedObject.FindProperty("_stripType");
            _foregroundImage = serializedObject.FindProperty("_foregroundImage");
            _shadowImage = serializedObject.FindProperty("_shadowImage");

            _valueType = serializedObject.FindProperty("_valueType");
            _valueLabel = serializedObject.FindProperty("_valueLabel");
            _valueFormat = serializedObject.FindProperty("_valueFormat");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawStrip();
            EditorGUILayout.Space();
            DrawValue();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStrip()
        {
            EditorGUILayout.PropertyField(_stripType);
            EditorGUILayout.PropertyField(_foregroundImage);

            var stripType = (StripType)_stripType.enumValueIndex;

            if (stripType == StripType.Shadow)
                EditorGUILayout.PropertyField(_shadowImage);
        }

        private void DrawValue()
        {
            EditorGUILayout.PropertyField(_valueType);

            var valueType = (ValueType)_valueType.enumValueIndex;

            if (valueType == ValueType.None)
                return;

            EditorGUILayout.PropertyField(_valueLabel);
            EditorGUILayout.PropertyField(_valueFormat);
        }
    }
}