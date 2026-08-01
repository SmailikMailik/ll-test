using LL.Game.Heroes;

namespace LL.Game.Ranks.Services
{
    internal interface IHeroExperienceService
    {
        int GetApplicableExperience(HeroId heroId, int amount);
        bool CanGrant(HeroId heroId, int amount);
        bool TryGrant(HeroId heroId, int amount);
    }
}