using LL.Game.Heroes;

namespace LL.User.State.Heroes
{
    internal interface IUserHeroProgressCommands
    {
        bool TryAddExperience(HeroId heroId, int amount);
        bool TryRankUp(HeroId heroId);
    }
}