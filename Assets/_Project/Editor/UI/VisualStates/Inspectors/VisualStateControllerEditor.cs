using LL.UI.VisualStates.Core;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace LLEditor.UI.VisualStates.Inspectors
{
    [CustomEditor(typeof(VisualStateController))]
    internal sealed class VisualStateControllerEditor : OdinEditor
    {
        private const int MaximumColumns = 3;
        private const float Padding = 6f;
        private const float Spacing = 4f;
        private const float HeaderHeight = 18f;
        private const float ButtonHeight = 20f;
        private const float HelpBoxHeight = 34f;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.Space(4f);
            DrawStatePreview();
        }

        public override bool RequiresConstantRepaint() => Application.isPlaying;

        private void DrawStatePreview()
        {
            var controller = (VisualStateController)target;
            var stateSet = controller.StateSet;
            var stateCount = stateSet == null ? 0 : stateSet.States.Count;
            var height = CalculatePreviewHeight(stateCount);
            var previewRect = GUILayoutUtility.GetRect(
                0f,
                height,
                GUILayout.ExpandWidth(true));

            if (stateCount == 0)
                return;

            GUI.Box(previewRect, GUIContent.none, EditorStyles.helpBox);

            var contentRect = new Rect(
                previewRect.x + Padding,
                previewRect.y + Padding,
                previewRect.width - Padding * 2f,
                previewRect.height - Padding * 2f);

            var headerRect = new Rect(
                contentRect.x,
                contentRect.y,
                contentRect.width,
                HeaderHeight);

            GUI.Label(headerRect, "State Preview", EditorStyles.boldLabel);

            var buttonAreaY = headerRect.yMax + Spacing;
            using (new EditorGUI.DisabledScope(Application.isPlaying is false))
            {
                DrawStateButtons(
                    controller,
                    stateSet,
                    contentRect,
                    buttonAreaY);
            }

            if (Application.isPlaying is false)
            {
                var rowCount = Mathf.CeilToInt((float)stateCount / MaximumColumns);
                var buttonsHeight = rowCount * ButtonHeight + (rowCount - 1) * Spacing;
                var helpRect = new Rect(
                    contentRect.x,
                    buttonAreaY + buttonsHeight + Spacing,
                    contentRect.width,
                    HelpBoxHeight);

                EditorGUI.HelpBox(
                    helpRect,
                    "Enter Play Mode to preview states.",
                    MessageType.Info);
            }
        }

        private static float CalculatePreviewHeight(int stateCount)
        {
            if (stateCount == 0)
                return 0f;

            var rowCount = Mathf.CeilToInt((float)stateCount / MaximumColumns);
            var buttonsHeight = rowCount * ButtonHeight + (rowCount - 1) * Spacing;
            var helpHeight = Application.isPlaying
                ? 0f
                : Spacing + HelpBoxHeight;

            return Padding * 2f +
                   HeaderHeight +
                   Spacing +
                   buttonsHeight +
                   helpHeight;
        }

        private static void DrawStateButtons(
            VisualStateController controller,
            VisualStateSet stateSet,
            Rect contentRect,
            float startY)
        {
            var stateCount = stateSet.States.Count;
            var columnCount = Mathf.Min(MaximumColumns, stateCount);
            var buttonWidth =
                (contentRect.width - (columnCount - 1) * Spacing) /
                columnCount;

            for (var index = 0; index < stateCount; index++)
            {
                var row = index / columnCount;
                var column = index % columnCount;
                var buttonRect = new Rect(
                    contentRect.x + column * (buttonWidth + Spacing),
                    startY + row * (ButtonHeight + Spacing),
                    buttonWidth,
                    ButtonHeight);

                var state = stateSet.States[index];
                var isSelected = state.Id == controller.CurrentState;
                var selected = GUI.Toggle(
                    buttonRect,
                    isSelected,
                    state.Name,
                    EditorStyles.miniButton);

                if (selected && isSelected is false)
                    controller.SetState(state.Id);
            }
        }
    }
}