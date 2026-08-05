using System;
using Sirenix.OdinInspector;

namespace LL.UI.StateRendering.Inspector
{
    [IncludeMyAttributes]
    [TableList(AlwaysExpanded = true, DrawScrollView = false, HideToolbar = true, IsReadOnly = true)]
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class StateValuesTableAttribute : Attribute { }
}