using System;
using System.Linq;
using LL.Game.Flags;
using LL.Infrastructure.Loading;
using LL.Presentation.Flags.Configuration;
using LL.Presentation.Sprites;
using LL.Validation;

namespace LL.Presentation.Flags.Loading
{
    internal sealed class ScriptableObjectFlagCatalogLoader : IDataLoader<SpriteCatalog<FlagId>>
    {
        private readonly FlagCatalogConfig _config;

        internal ScriptableObjectFlagCatalogLoader(FlagCatalogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public SpriteCatalog<FlagId> Load()
        {
            ValidationRunner.EnsureValid(_config);
            return new SpriteCatalog<FlagId>(_config.Flags.Select(entry => entry.ToPair()));
        }
    }
}