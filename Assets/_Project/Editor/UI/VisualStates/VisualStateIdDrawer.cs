using LL.UI.VisualStates;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.VisualStates
{
    [CustomPropertyDrawer(typeof(VisualStateId))]
    internal sealed class VisualStateIdDrawer : PropertyDrawer
    {
        private const string EmptyOption = "Choose State...";
        private static readonly GUIContent[] _emptyOptions = { new(EmptyOption) };

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("_value");
            var stateSet = ResolveStateSet(property.serializedObject);

            EditorGUI.BeginProperty(position, label, property);

            if (valueProperty == null || stateSet == null || stateSet.States.Count == 0)
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUI.Popup(position, label, 0, _emptyOptions);

                EditorGUI.EndProperty();
                return;
            }

            var options = new GUIContent[stateSet.States.Count + 1];
            var selectedIndex = 0;

            options[0] = new GUIContent(EmptyOption);

            for (var index = 0; index < stateSet.States.Count; index++)
            {
                var state = stateSet.States[index];
                var optionIndex = index + 1;

                options[optionIndex] = new GUIContent(state.Name);

                if (state.Id.ToString() == valueProperty.stringValue)
                    selectedIndex = optionIndex;
            }

            EditorGUI.showMixedValue = valueProperty.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();

            var nextIndex = EditorGUI.Popup(position, label, selectedIndex, options);

            if (EditorGUI.EndChangeCheck())
            {
                valueProperty.stringValue = nextIndex == 0
                    ? string.Empty
                    : stateSet.States[nextIndex - 1].Id.ToString();
            }

            EditorGUI.showMixedValue = false;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUIUtility.singleLineHeight;

        private static VisualStateSet ResolveStateSet(SerializedObject serializedObject)
        {
            if (serializedObject.targetObject is VisualStateController controller)
                return controller.StateSet;

            var controllerProperty = serializedObject.FindProperty("_controller");

            return controllerProperty?.objectReferenceValue is VisualStateController linkedController
                ? linkedController.StateSet
                : null;
        }
    }
}