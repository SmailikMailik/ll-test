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
                ToUserProgressDefaultDeclaration(_config.Progress),
                _config.Items.Select(ToUserItemDefaultDeclaration));
        }

        private static UserIdentityDefaultDeclaration ToUserIdentityDefaultDeclaration(UserIdentityDefaults identity)
        {
            return new UserIdentityDefaultDeclaration(identity.UserId, identity.RegionCode);
        }

        private static UserProgressDefaultDeclaration ToUserProgressDefaultDeclaration(UserProgressDefaults progress)
        {
            return new UserProgressDefaultDeclaration(progress.RankId.Value, progress.Experience);
        }

        private static UserItemDefaultDeclaration ToUserItemDefaultDeclaration(UserItemDefaultEntry item)
        {
            return new UserItemDefaultDeclaration(item.Id.Value, item.Amount);
        }
    }
}