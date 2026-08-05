using System;
using System.Linq;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Presentation.Items.Configuration;
using LL.Presentation.Sprites;
using LL.Validation;

namespace LL.Presentation.Items.Loading
{
    internal sealed class ScriptableObjectItemIconCatalogLoader : IDataLoader<SpriteCatalog<ItemId>>
    {
        private readonly ItemIconCatalogConfig _config;

        internal ScriptableObjectItemIconCatalogLoader(ItemIconCatalogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public SpriteCatalog<ItemId> Load()
        {
            ValidationRunner.EnsureValid(_config);

            return new SpriteCatalog<ItemId>(_config.Icons.Select(entry => entry.ToPair()));
        }
    }
}