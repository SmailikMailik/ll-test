using System;
using LL.Game.Countries;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Presentation.Heroes;
using LL.Presentation.Icons;
using LL.Presentation.Localization;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class PresentationInstaller : IInstaller
    {
        private readonly IDataLoader<IconCatalog<ItemId>> _itemIconCatalogLoader;
        private readonly IDataLoader<IconCatalog<CountryId>> _countryFlagCatalogLoader;
        private readonly IDataLoader<HeroPortraitCatalog> _heroPortraitCatalogLoader;

        internal PresentationInstaller(
            IDataLoader<IconCatalog<ItemId>> itemIconCatalogLoader,
            IDataLoader<IconCatalog<CountryId>> countryFlagCatalogLoader,
            IDataLoader<HeroPortraitCatalog> heroPortraitCatalogLoader)
        {
            _itemIconCatalogLoader = itemIconCatalogLoader ?? throw new ArgumentNullException(nameof(itemIconCatalogLoader));
            _countryFlagCatalogLoader = countryFlagCatalogLoader ?? throw new ArgumentNullException(nameof(countryFlagCatalogLoader));
            _heroPortraitCatalogLoader = heroPortraitCatalogLoader ?? throw new ArgumentNullException(nameof(heroPortraitCatalogLoader));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.Register<UnityLocalizationService>(Lifetime.Singleton).As<ILocalizationService>();
            builder.RegisterLoadedData(_itemIconCatalogLoader);
            builder.RegisterLoadedData(_countryFlagCatalogLoader);
            builder.RegisterLoadedData(_heroPortraitCatalogLoader);
        }
    }
}