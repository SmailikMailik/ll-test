using System;
using LL.Game.Flags;
using LL.Game.Heroes;
using LL.Presentation.Localization;
using LL.Presentation.Sprites;
using LL.User.Snapshots;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Views.Heroes
{
    [DisallowMultipleComponent]
    internal sealed class HeroIdentityView : MonoBehaviour
    {
        [SerializeField] private Image _flagImage;
        [SerializeField] private TMP_Text _nameLabel;

        private HeroCatalog _heroes;
        private SpriteCatalog<FlagId> _flags;
        private ILocalizationService _localization;
        private UserHeroSelectionSnapshot _heroSelection;

        private HeroId _heroId;
        private bool _hasHero;

        [Inject]
        private void Construct(
            HeroCatalog heroes,
            SpriteCatalog<FlagId> flags,
            ILocalizationService localization,
            UserHeroSelectionSnapshot heroSelection)
        {
            _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
            _flags = flags ?? throw new ArgumentNullException(nameof(flags));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            _heroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
        }

        private void Start()
        {
            _localization.LocaleChanged.Subscribe(_ => OnLocaleChanged()).AddTo(this);

            if (_hasHero is false)
                Show(_heroSelection.HeroId);
        }

        internal void Show(HeroId heroId)
        {
            var hero = _heroes.GetHero(heroId);

            if (_flags.TryGetSprite(hero.FlagId, out var flag) is false)
                throw new InvalidOperationException($"Flag '{hero.FlagId}' is not configured.");

            _heroId = heroId;
            _hasHero = true;

            _flagImage.sprite = flag;
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