using System;
using LL.Game.Countries;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Presentation.Heroes;
using LL.Presentation.Localization;
using LL.Presentation.Sprites;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class PresentationInstaller : IInstaller
    {
        private readonly IDataLoader<SpriteCatalog<ItemId>> _itemIconCatalogLoader;
        private readonly IDataLoader<SpriteCatalog<CountryId>> _countryFlagCatalogLoader;
        private readonly IDataLoader<SpriteVariantCatalog<HeroId, HeroPortraitSize>> _heroPortraitCatalogLoader;

        internal PresentationInstaller(
            IDataLoader<SpriteCatalog<ItemId>> itemIconCatalogLoader,
            IDataLoader<SpriteCatalog<CountryId>> countryFlagCatalogLoader,
            IDataLoader<SpriteVariantCatalog<HeroId, HeroPortraitSize>> heroPortraitCatalogLoader)
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