using System;
using System.Linq;
using LL.User.Configuration;
using LL.User.Defaults.Declarations;
using LL.Validation;

namespace LL.User.Defaults.Sources
{
    internal sealed class ScriptableObjectUserDefaultsSource : IUserDefaultsSource
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
                _config.Identity.UserId,
                _config.Identity.RegionCode,
                _config.Progress.RankId.Value,
                _config.Progress.Experience,
                _config.Items.Select(item => new UserItemDefaultsDeclaration(item.Id.Value, item.Amount)));
        }
    }
}