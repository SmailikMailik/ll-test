using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LLEditor.Toolbar
{
    internal static class MainToolbar
    {
        private const string PlayModeZoneName = "ToolbarZonePlayMode";

        private static readonly Type _toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly FieldInfo _rootField = _toolbarType?.GetField("m_Root", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static bool TryAttachToPlayModeZone(VisualElement element)
        {
            if (_toolbarType == null || _rootField == null)
                return false;

            var toolbar = Resources.FindObjectsOfTypeAll(_toolbarType).FirstOrDefault();

            if (toolbar == null)
                return false;

            var root = _rootField.GetValue(toolbar) as VisualElement;
            var playModeZone = root?.Q(PlayModeZoneName);

            if (playModeZone == null)
                return false;

            playModeZone.Q(element.name)?.RemoveFromHierarchy();
            playModeZone.Insert(0, element);
            return true;
        }
    }
}