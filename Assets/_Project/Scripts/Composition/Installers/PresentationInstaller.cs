using System;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Presentation.Icons;
using LL.Presentation.Localization;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class PresentationInstaller : IInstaller
    {
        private readonly IDataLoader<IconCatalog<ItemId>> _itemIconCatalogLoader;

        internal PresentationInstaller(IDataLoader<IconCatalog<ItemId>> itemIconCatalogLoader)
        {
            _itemIconCatalogLoader = itemIconCatalogLoader ?? throw new ArgumentNullException(nameof(itemIconCatalogLoader));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.Register<UnityLocalizationService>(Lifetime.Singleton).As<ILocalizationService>();
            builder.RegisterLoadedData(_itemIconCatalogLoader);
        }
    }
}