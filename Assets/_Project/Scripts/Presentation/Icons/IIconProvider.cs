using LL.Identifiers;
using UnityEngine;

namespace LL.Presentation.Icons
{
    internal interface IIconProvider<in TId> where TId : struct, IIdentifier
    {
        bool TryGetIcon(TId id, out Sprite icon);
    }
}