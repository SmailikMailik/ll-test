using System;
using System.Linq;
using LL.Game.Heroes;
using LL.Infrastructure.Loading;
using LL.Presentation.Heroes.Configuration;
using LL.Presentation.Sprites;
using LL.Validation;

namespace LL.Presentation.Heroes.Loading
{
    internal sealed class ScriptableObjectHeroPortraitCatalogLoader :
        IDataLoader<SpriteVariantCatalog<HeroId, HeroPortraitSize>>
    {
        private readonly HeroPortraitCatalogConfig _config;

        internal ScriptableObjectHeroPortraitCatalogLoader(HeroPortraitCatalogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public SpriteVariantCatalog<HeroId, HeroPortraitSize> Load()
        {
            ValidationRunner.EnsureValid(_config);
            return new SpriteVariantCatalog<HeroId, HeroPortraitSize>(
                _config.Portraits.SelectMany(entry => entry.ToVariants()));
        }
    }
}