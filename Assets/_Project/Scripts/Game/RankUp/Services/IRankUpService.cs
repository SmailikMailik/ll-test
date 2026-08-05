using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Items;

namespace LL.Game.RankUp.Services
{
    internal interface IRankUpService
    {
        bool TryGetDefinition(HeroId heroId, out RankUpDefinition definition);
        bool TryActivateOption(HeroId heroId, RankUpOptionId optionId);
        bool CanCompleteOption(HeroId heroId, RankUpOptionId optionId);

        bool TryCompleteOption(
            HeroId heroId,
            RankUpOptionId optionId,
            out IReadOnlyList<ItemAmount> rewardItems);
    }
}