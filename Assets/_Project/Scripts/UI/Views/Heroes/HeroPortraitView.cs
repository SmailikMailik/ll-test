using System;
using LL.Game.Heroes;
using LL.Presentation.Heroes;
using LL.Presentation.Sprites;
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
        [SerializeField] private string _initialHeroId;

        private SpriteVariantCatalog<HeroId, HeroPortraitSize> _portraits;
        private bool _hasHero;

        [Inject]
        private void Construct(SpriteVariantCatalog<HeroId, HeroPortraitSize> portraits)
        {
            _portraits = portraits ?? throw new ArgumentNullException(nameof(portraits));
        }

        private void Start()
        {
            if (_hasHero is false && string.IsNullOrWhiteSpace(_initialHeroId) is false)
                Show(new HeroId(_initialHeroId));
        }

        internal void Show(HeroId heroId)
        {
            _image.sprite = _portraits.GetSprite(heroId, _size);
            _image.enabled = true;
            _hasHero = true;
        }

        internal void Clear()
        {
            _image.sprite = null;
            _image.enabled = false;
            _hasHero = false;
        }
    }
}