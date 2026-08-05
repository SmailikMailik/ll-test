using System;
using Sirenix.OdinInspector;

namespace LL.Presentation.Inspector
{
    [IncludeMyAttributes]
    [PreviewField(48, ObjectFieldAlignment.Center)]
    [TableColumnWidth(64)]
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class SpritePreviewAttribute : Attribute { }
}