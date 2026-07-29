using System;
using System.Collections.Generic;

namespace LL.Game.Items
{
    internal static class ItemIds
    {
        internal static readonly ItemId Soft = new("soft");
        internal static readonly ItemId Hard = new("hard");
        internal static readonly ItemId MasterPoint = new("master_point");
        internal static readonly ItemId Skill = new("skill");
        internal static readonly ItemId Slot = new("slot");

        internal static IReadOnlyList<ItemId> All { get; } = Array.AsReadOnly(
            new[]
            {
                Soft,
                Hard,
                MasterPoint,
                Skill,
                Slot
            });
    }
}