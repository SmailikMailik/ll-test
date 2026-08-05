using LL.Game.Heroes;
using LL.Game.Identifiers;

namespace LL.User.Snapshots
{
    internal sealed class UserHeroSelectionSnapshot
    {
        internal HeroId HeroId { get; }

        internal UserHeroSelectionSnapshot(HeroId heroId)
        {
            IdentifierValidator.EnsureValid(heroId, nameof(heroId));

            HeroId = heroId;
        }
    }
}