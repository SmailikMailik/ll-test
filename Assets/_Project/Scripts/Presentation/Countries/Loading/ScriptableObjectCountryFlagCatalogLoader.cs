using System;
using System.Linq;
using LL.Game.Countries;
using LL.Infrastructure.Loading;
using LL.Presentation.Countries.Configuration;
using LL.Presentation.Icons;
using LL.Validation;

namespace LL.Presentation.Countries.Loading
{
    internal sealed class ScriptableObjectCountryFlagCatalogLoader : IDataLoader<IconCatalog<CountryId>>
    {
        private readonly CountryFlagCatalogConfig _config;

        internal ScriptableObjectCountryFlagCatalogLoader(CountryFlagCatalogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IconCatalog<CountryId> Load()
        {
            ValidationRunner.EnsureValid(_config);
            return new IconCatalog<CountryId>(_config.Flags.Select(entry => entry.ToPair()));
        }
    }
}