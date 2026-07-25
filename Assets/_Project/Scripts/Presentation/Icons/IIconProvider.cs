using UnityEngine;

namespace LL.Presentation.Icons
{
    internal interface IIconProvider<TId> where TId : struct
    {
        bool TryGetIcon(TId id, out Sprite icon);
    }
}