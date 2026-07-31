using System;
using System.Linq;
using LL.Infrastructure.Loading;
using LL.Presentation.Heroes.Configuration;
using LL.Validation;

namespace LL.Presentation.Heroes.Loading
{
    internal sealed class ScriptableObjectHeroPortraitCatalogLoader : IDataLoader<HeroPortraitCatalog>
    {
        private readonly HeroPortraitCatalogConfig _config;

        internal ScriptableObjectHeroPortraitCatalogLoader(HeroPortraitCatalogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public HeroPortraitCatalog Load()
        {
            ValidationRunner.EnsureValid(_config);
            return new HeroPortraitCatalog(_config.Portraits.Select(entry => entry.ToDefinition()));
        }
    }
}