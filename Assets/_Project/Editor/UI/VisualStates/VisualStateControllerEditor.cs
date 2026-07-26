using LL.UI.VisualStates;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.VisualStates
{
    [CustomEditor(typeof(VisualStateController))]
    internal sealed class VisualStateControllerEditor : OdinEditor
    {
        private const int MaximumColumns = 3;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space(4f);
            DrawStatePreview();
        }

        public override bool RequiresConstantRepaint() => Application.isPlaying;

        private void DrawStatePreview()
        {
            var controller = (VisualStateController)target;
            var stateSet = controller.StateSet;

            if (stateSet == null || stateSet.States.Count == 0)
                return;

            var names = new string[stateSet.States.Count];
            var currentIndex = -1;

            for (var index = 0; index < stateSet.States.Count; index++)
            {
                var state = stateSet.States[index];
                names[index] = state.Name;

                if (state.Id == controller.CurrentState)
                    currentIndex = index;
            }

            SirenixEditorGUI.BeginBox("State Preview");

            using (new EditorGUI.DisabledScope(Application.isPlaying is false))
            {
                var columnCount = Mathf.Min(MaximumColumns, names.Length);
                var selectedIndex = GUILayout.SelectionGrid(
                    currentIndex,
                    names,
                    columnCount);

                if (selectedIndex != currentIndex)
                    controller.SetState(stateSet.States[selectedIndex].Id);
            }

            if (Application.isPlaying is false)
            {
                EditorGUILayout.HelpBox(
                    "Enter Play Mode to preview states.",
                    MessageType.Info);
            }

            SirenixEditorGUI.EndBox();
        }
    }
}