using System;
using LL.Game.Heroes;
using LL.Presentation.Heroes;
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

        private HeroPortraitCatalog _portraits;
        private bool _hasHero;

        [Inject]
        private void Construct(HeroPortraitCatalog portraits)
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
            _image.sprite = _portraits.GetPortrait(heroId, _size);
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