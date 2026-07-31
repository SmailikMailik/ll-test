using System.Collections.Generic;
using LL.Game.Items;

namespace LL.Game.Upgrades.Services
{
    internal interface ICardExperienceService
    {
        bool TryGetApplication(IReadOnlyList<ItemAmount> cards, out ExperienceApplication application);
        bool TryApply(IReadOnlyList<ItemAmount> cards);
    }
}