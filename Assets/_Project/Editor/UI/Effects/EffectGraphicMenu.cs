using LL.UI.Effects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LLEditor.UI.Effects
{
    internal static class EffectGraphicMenu
    {
        private const int MenuPriority = 2048;
        private static readonly Vector2 _defaultSize = new(100f, 100f);

        [MenuItem("GameObject/UI/Gradient", false, MenuPriority)]
        private static void CreateGradient(MenuCommand menuCommand)
        {
            CreateGraphic<GradientGraphic>("Gradient", menuCommand);
        }

        [MenuItem("GameObject/UI/Border", false, MenuPriority + 1)]
        private static void CreateBorder(MenuCommand menuCommand)
        {
            CreateGraphic<BorderGraphic>("Border", menuCommand);
        }

        [MenuItem("GameObject/UI/Rounded Rectangle", false, MenuPriority + 2)]
        private static void CreateRoundedRectangle(MenuCommand menuCommand)
        {
            CreateGraphic<RoundedRectangleGraphic>("Rounded Rectangle", menuCommand);
        }

        private static void CreateGraphic<TGraphic>(string name, MenuCommand menuCommand)
            where TGraphic : MaskableGraphic
        {
            var graphicObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TGraphic));
            var parent = menuCommand.context as GameObject;

            GameObjectUtility.SetParentAndAlign(graphicObject, parent);
            Undo.RegisterCreatedObjectUndo(graphicObject, $"Create {name}");

            var rectTransform = (RectTransform)graphicObject.transform;
            rectTransform.sizeDelta = _defaultSize;
            rectTransform.anchoredPosition = Vector2.zero;
            graphicObject.GetComponent<TGraphic>().raycastTarget = false;

            var uiLayer = LayerMask.NameToLayer("UI");

            if (uiLayer >= 0)
                graphicObject.layer = uiLayer;

            Selection.activeGameObject = graphicObject;
        }
    }
}