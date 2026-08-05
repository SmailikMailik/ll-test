using System;
using LL.Game.Heroes;
using LL.User.State.Heroes;
using VContainer;

namespace LL.Game.Ranks.Services
{
    internal sealed class HeroExperienceService : IHeroExperienceService
    {
        private readonly IUserHeroProgress _progress;
        private readonly IUserHeroProgressCommands _commands;

        [Inject]
        internal HeroExperienceService(
            IUserHeroProgress progress,
            IUserHeroProgressCommands commands)
        {
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        public int GetApplicableExperience(HeroId heroId, int amount) =>
            _progress.GetApplicableExperience(heroId, amount);

        public bool CanGrant(HeroId heroId, int amount) =>
            _progress.CanAddExperience(heroId, amount);

        public bool TryGrant(HeroId heroId, int amount) =>
            _commands.TryAddExperience(heroId, amount);
    }
}