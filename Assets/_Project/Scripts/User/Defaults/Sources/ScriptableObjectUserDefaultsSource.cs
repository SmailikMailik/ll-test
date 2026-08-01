using System;
using System.Linq;
using LL.User.Configuration;
using LL.User.Defaults.Declarations;
using LL.Infrastructure.Loading;
using LL.Validation;

namespace LL.User.Defaults.Sources
{
    internal sealed class ScriptableObjectUserDefaultsSource : IDataSource<UserDefaultsDeclaration>
    {
        private readonly UserDefaultsConfig _config;

        internal ScriptableObjectUserDefaultsSource(UserDefaultsConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public UserDefaultsDeclaration Read()
        {
            ValidationRunner.EnsureValid(_config);

            return new UserDefaultsDeclaration(
                ToUserIdentityDefaultDeclaration(_config.Identity),
                ToUserHeroSelectionDefaultDeclaration(_config.HeroSelection),
                _config.Heroes.Select(ToUserHeroDefaultDeclaration),
                _config.Items.Select(ToUserItemDefaultDeclaration));
        }

        private static UserIdentityDefaultDeclaration ToUserIdentityDefaultDeclaration(UserIdentityDefaults identity)
        {
            return new UserIdentityDefaultDeclaration(identity.UserId, identity.RegionCode);
        }

        private static UserHeroSelectionDefaultDeclaration ToUserHeroSelectionDefaultDeclaration(
            UserHeroSelectionDefaults heroSelection)
        {
            return new UserHeroSelectionDefaultDeclaration(heroSelection.HeroId.Value);
        }

        private static UserHeroDefaultDeclaration ToUserHeroDefaultDeclaration(UserHeroDefaultEntry hero)
        {
            return new UserHeroDefaultDeclaration(
                hero.HeroId.Value,
                hero.RankId.Value,
                hero.Experience);
        }

        private static UserItemDefaultDeclaration ToUserItemDefaultDeclaration(UserItemDefaultEntry item)
        {
            return new UserItemDefaultDeclaration(item.Id.Value, item.Amount);
        }
    }
}