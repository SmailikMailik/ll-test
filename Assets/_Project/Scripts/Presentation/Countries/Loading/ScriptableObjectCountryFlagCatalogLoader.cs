using System;
using System.Linq;
using LL.Game.Countries;
using LL.Infrastructure.Loading;
using LL.Presentation.Countries.Configuration;
using LL.Presentation.Sprites;
using LL.Validation;

namespace LL.Presentation.Countries.Loading
{
    internal sealed class ScriptableObjectCountryFlagCatalogLoader : IDataLoader<SpriteCatalog<CountryId>>
    {
        private readonly CountryFlagCatalogConfig _config;

        internal ScriptableObjectCountryFlagCatalogLoader(CountryFlagCatalogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public SpriteCatalog<CountryId> Load()
        {
            ValidationRunner.EnsureValid(_config);
            return new SpriteCatalog<CountryId>(_config.Flags.Select(entry => entry.ToPair()));
        }
    }
}