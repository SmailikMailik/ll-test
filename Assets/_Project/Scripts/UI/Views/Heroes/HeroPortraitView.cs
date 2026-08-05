using System;
using LL.Game.Heroes;
using LL.Presentation.Heroes;
using LL.Presentation.Sprites;
using LL.User.Snapshots;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace LL.UI.Views.Heroes
{
    [DisallowMultipleComponent]
    internal sealed class HeroPortraitView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private HeroPortraitSize _size;

        private SpriteVariantCatalog<HeroId, HeroPortraitSize> _portraits;
        private UserHeroSelectionSnapshot _heroSelection;

        private bool _hasHero;

        [Inject]
        private void Construct(
            SpriteVariantCatalog<HeroId, HeroPortraitSize> portraits,
            UserHeroSelectionSnapshot heroSelection)
        {
            _portraits = portraits ?? throw new ArgumentNullException(nameof(portraits));
            _heroSelection = heroSelection ?? throw new ArgumentNullException(nameof(heroSelection));
        }

        private void Start()
        {
            if (_hasHero is false)
                Show(_heroSelection.HeroId);
        }

        internal void Show(HeroId heroId)
        {
            _image.sprite = _portraits.GetSprite(heroId, _size);
            _hasHero = true;
        }
    }
}