using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Heroes.Configuration
{
    [CreateAssetMenu(fileName = nameof(HeroPortraitCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class HeroPortraitCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidPortraits), "Hero portrait data is invalid.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private HeroPortraitEntry[] _portraits;

        internal const string CreationPath = "LL/Presentation/Hero Portrait Catalog";

        private static readonly IDataValidator<HeroPortraitEntry[]> _validator = new HeroPortraitCatalogConfigValidator();

        internal IReadOnlyList<HeroPortraitEntry> Portraits => _portraits;

        private static bool HasValidPortraits(HeroPortraitEntry[] portraits) => ValidationRunner.IsValid(portraits, _validator);

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_portraits, context);
        }
    }

    [Serializable]
    internal sealed class HeroPortraitEntry
    {
        [LabelText("Hero ID")]
        [SerializeField] private string _heroId;

        [SerializeField, Required, PreviewField(64, ObjectFieldAlignment.Center)] private Sprite _smallPortrait;
        [SerializeField, Required, PreviewField(128, ObjectFieldAlignment.Center)] private Sprite _largePortrait;

        internal HeroId HeroId => new(_heroId);
        internal Sprite SmallPortrait => _smallPortrait;
        internal Sprite LargePortrait => _largePortrait;

        internal HeroPortraitDefinition ToDefinition() => new(HeroId, SmallPortrait, LargePortrait);
    }
}