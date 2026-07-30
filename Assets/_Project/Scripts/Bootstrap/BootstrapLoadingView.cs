using UnityEngine;
using UnityEngine.UI;

namespace LL.Bootstrap
{
    internal sealed class BootstrapLoadingView
    {
        internal Image ProgressFill { get; }
        internal Text LoadingLabel { get; }

        private BootstrapLoadingView(Image progressFill, Text loadingLabel)
        {
            ProgressFill = progressFill;
            LoadingLabel = loadingLabel;
        }

        internal static BootstrapLoadingView Create()
        {
            var canvasObject = new GameObject(
                "Canvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var background = CreateImage(
                "Image_Background",
                canvasObject.transform,
                new Color(0.035f, 0.047f, 0.075f, 1f));
            Stretch(background.rectTransform);

            var title = CreateLabel(
                "Label_Title",
                background.transform,
                "LAST LEVEL",
                46,
                FontStyle.Bold,
                new Color(0.9f, 0.93f, 1f, 1f));
            SetAnchoredRect(title.rectTransform, new Vector2(0.5f, 0.58f), new Vector2(720f, 80f));

            var loadingLabel = CreateLabel(
                "Label_Loading",
                background.transform,
                "LOADING 0%",
                22,
                FontStyle.Normal,
                new Color(0.63f, 0.7f, 0.84f, 1f));
            SetAnchoredRect(loadingLabel.rectTransform, new Vector2(0.5f, 0.48f), new Vector2(640f, 44f));

            var bar = CreateImage(
                "Image_ProgressBackground",
                background.transform,
                new Color(0.11f, 0.14f, 0.21f, 1f));
            SetAnchoredRect(bar.rectTransform, new Vector2(0.5f, 0.42f), new Vector2(640f, 18f));

            var fill = CreateImage(
                "Image_ProgressFill",
                bar.transform,
                new Color(0.25f, 0.64f, 1f, 1f));
            Stretch(fill.rectTransform, 3f);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 0f;

            return new BootstrapLoadingView(fill, loadingLabel);
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            gameObject.transform.SetParent(parent, false);

            var image = gameObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Text CreateLabel(
            string name,
            Transform parent,
            string text,
            int fontSize,
            FontStyle fontStyle,
            Color color)
        {
            var gameObject = new GameObject(
                name,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Text));
            gameObject.transform.SetParent(parent, false);

            var label = gameObject.GetComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.color = color;
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;
            return label;
        }

        private static void SetAnchoredRect(
            RectTransform rectTransform,
            Vector2 anchor,
            Vector2 size)
        {
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = size;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        private static void Stretch(RectTransform rectTransform, float inset = 0f)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(-inset * 2f, -inset * 2f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
    }
}