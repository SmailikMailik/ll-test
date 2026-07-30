using System;
using LL.Presentation.Icons.Configuration;
using LL.Presentation.Localization;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class PresentationInstaller : IInstaller
    {
        private readonly ItemIconCatalogConfig _itemIconCatalogConfig;

        internal PresentationInstaller(ItemIconCatalogConfig itemIconCatalogConfig)
        {
            _itemIconCatalogConfig = itemIconCatalogConfig ?? throw new ArgumentNullException(nameof(itemIconCatalogConfig));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.Register<UnityLocalizationService>(Lifetime.Singleton).As<ILocalizationService>();
            builder.RegisterLoadedData(_itemIconCatalogConfig);
        }
    }
}