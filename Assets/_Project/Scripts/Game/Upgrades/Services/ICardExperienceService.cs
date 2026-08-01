using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Items;

namespace LL.Game.Upgrades.Services
{
    internal interface ICardExperienceService
    {
        bool TryGetApplication(
            HeroId heroId,
            IReadOnlyList<ItemAmount> cards,
            out ExperienceApplication application);

        bool TryApply(HeroId heroId, IReadOnlyList<ItemAmount> cards);
    }
}