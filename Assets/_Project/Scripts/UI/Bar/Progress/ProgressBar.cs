using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Bar.Progress
{
    internal sealed class ProgressBar : MonoBehaviour
    {
        [SerializeField] private StripType _stripType = StripType.Common;
        [SerializeField] private Image _foregroundImage;
        [SerializeField] private Image _shadowImage;

        [SerializeField] private ValueType _valueType = ValueType.None;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private string _valueFormat = "{0} / {1}";

        private IStrip _strip;
        private IValue _value;

        internal void Awake()
        {
            var strips = new Dictionary<StripType, IStrip>
            {
                [StripType.Common] = new StripCommon(_foregroundImage),
                [StripType.Shadow] = new StripShadow(this, _foregroundImage, _shadowImage)
            };

            var values = new Dictionary<ValueType, IValue>
            {
                [ValueType.None] = new ValueMock(),
                [ValueType.Integer] = new ValueInteger(_valueLabel, _valueFormat),
                [ValueType.Floating] = new ValueFloating(_valueLabel, _valueFormat),
                [ValueType.Percent] = new ValuePercent(_valueLabel, _valueFormat)
            };

            _strip = strips[_stripType];
            _value = values[_valueType];
        }

        internal void SetValue(float value, float minValue, float maxValue)
        {
            value = Mathf.Clamp(value, minValue, maxValue);

            _strip.SetFillAmount(value, maxValue);
            _value.SetText(value, maxValue);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ProgressBar))]
    public class ProgressBarEditor : Editor
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
#endif
}