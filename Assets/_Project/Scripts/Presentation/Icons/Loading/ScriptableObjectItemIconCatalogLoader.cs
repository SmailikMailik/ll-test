using System;
using System.Linq;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Presentation.Icons.Configuration;
using LL.Validation;

namespace LL.Presentation.Icons.Loading
{
    internal sealed class ScriptableObjectItemIconCatalogLoader : IDataLoader<IconCatalog<ItemId>>
    {
        private readonly ItemIconCatalogConfig _config;

        internal ScriptableObjectItemIconCatalogLoader(ItemIconCatalogConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IconCatalog<ItemId> Load()
        {
            ValidationRunner.EnsureValid(_config);

            return new IconCatalog<ItemId>(_config.Icons.Select(entry => entry.ToPair()));
        }
    }
}