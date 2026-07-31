using System;
using System.Collections.Generic;
using LL.Game.Flags;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Heroes.Configuration
{
    [CreateAssetMenu(fileName = nameof(HeroCatalogConfig), menuName = CreationPath)]
    internal sealed class HeroCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidHeroes), "Hero catalog data is invalid.")]
        [SerializeField] private HeroEntry[] _heroes;

        internal const string CreationPath = "LL/Game Data/Hero Catalog";

        private static readonly IDataValidator<HeroEntry[]> _validator = new HeroCatalogConfigValidator();

        internal IReadOnlyList<HeroEntry> Heroes => _heroes;

        private static bool HasValidHeroes(HeroEntry[] heroes) => ValidationRunner.IsValid(heroes, _validator);

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_heroes, context);
        }
    }

    [Serializable]
    internal sealed class HeroEntry
    {
        [SerializeField] private string _id;
        [SerializeField] private string _nameLocalizationKey;
        [SerializeField] private string _flagId;

        internal HeroId Id => new(_id);
        internal string NameLocalizationKey => _nameLocalizationKey;
        internal FlagId FlagId => new(_flagId);
    }
}