using System;
using LL.Game.Countries;
using LL.Game.Heroes;
using LL.Presentation.Icons;
using LL.Presentation.Localization;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Views.Heroes
{
    [DisallowMultipleComponent]
    internal sealed class HeroView : MonoBehaviour
    {
        [SerializeField] private Image _flagImage;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private string _initialHeroId;

        private HeroCatalog _heroes;
        private IconCatalog<CountryId> _flags;
        private ILocalizationService _localization;
        private HeroId _heroId;
        private bool _hasHero;

        [Inject]
        private void Construct(
            HeroCatalog heroes,
            IconCatalog<CountryId> flags,
            ILocalizationService localization)
        {
            _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
            _flags = flags ?? throw new ArgumentNullException(nameof(flags));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        private void Start()
        {
            _localization.LocaleChanged.Subscribe(_ => OnLocaleChanged()).AddTo(this);

            if (_hasHero is false && string.IsNullOrWhiteSpace(_initialHeroId) is false)
                Show(new HeroId(_initialHeroId));
        }

        internal void Show(HeroId heroId)
        {
            var hero = _heroes.GetHero(heroId);

            if (_flags.TryGetIcon(hero.CountryId, out var flag) is false)
                throw new InvalidOperationException($"Flag for country '{hero.CountryId}' is not configured.");

            _heroId = heroId;
            _hasHero = true;
            _flagImage.sprite = flag;
            _flagImage.enabled = true;
            RefreshName(hero);
        }

        private void OnLocaleChanged()
        {
            if (_hasHero)
                RefreshName(_heroes.GetHero(_heroId));
        }

        private void RefreshName(HeroDefinition hero)
        {
            _nameLabel.text = _localization.GetText(hero.NameLocalizationKey);
        }
    }
}