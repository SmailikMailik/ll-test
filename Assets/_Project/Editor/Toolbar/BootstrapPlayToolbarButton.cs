using LLEditor.Bootstrap;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LLEditor.Toolbar
{
    [InitializeOnLoad]
    internal static class BootstrapPlayToolbarButton
    {
        private const string ButtonText = "B";
        private const string ButtonName = "LastLevelBootstrapPlayButton";
        private const string ButtonTooltip = "Play Last Level from the Bootstrap scene.";

        private static readonly Button _button = CreateButton();

        static BootstrapPlayToolbarButton()
        {
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnEditorUpdate()
        {
            if (_button?.panel is not null)
                return;

            MainToolbar.TryAttachToPlayModeZone(_button);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange _)
        {
            _button.SetEnabled(BootstrapPlayMode.CanStart);
        }

        private static Button CreateButton()
        {
            var button = new Button(BootstrapPlayMode.Play)
            {
                name = ButtonName,
                text = ButtonText,
                tooltip = ButtonTooltip
            };
            button.AddToClassList("unity-toolbar-button");
            button.style.width = 32f;
            button.style.height = 20f;
            button.style.alignSelf = Align.Center;
            button.style.marginLeft = 2f;
            button.style.marginRight = 2f;
            button.style.paddingLeft = 0f;
            button.style.paddingRight = 0f;
            button.style.borderLeftWidth = 1f;
            button.style.borderLeftColor = new Color(0.25f, 0.64f, 1f);
            button.SetEnabled(BootstrapPlayMode.CanStart);
            return button;
        }
    }
}