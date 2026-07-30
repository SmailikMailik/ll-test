using System.Collections.Generic;
using LL.Game.Items;
using LL.Game.Upgrades;

namespace LL.Game.Upgrades.Services
{
    internal interface ICardExperienceService
    {
        bool TryGetApplication(IReadOnlyList<ItemAmount> cards, out ExperienceApplication application);
        bool TryApply(IReadOnlyList<ItemAmount> cards);
    }
}