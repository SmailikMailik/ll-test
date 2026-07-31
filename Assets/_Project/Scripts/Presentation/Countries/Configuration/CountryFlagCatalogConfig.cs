using System;
using System.Collections.Generic;
using LL.Game.Countries;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Presentation.Countries.Configuration
{
    [CreateAssetMenu(fileName = nameof(CountryFlagCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class CountryFlagCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidFlags), "Country flag data is invalid.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CountryFlagEntry[] _flags;

        internal const string CreationPath = "LL/Presentation/Country Flag Catalog";

        private static readonly IDataValidator<CountryFlagEntry[]> _validator = new CountryFlagCatalogConfigValidator();

        internal IReadOnlyList<CountryFlagEntry> Flags => _flags;

        private static bool HasValidFlags(CountryFlagEntry[] flags) => ValidationRunner.IsValid(flags, _validator);

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_flags, context);
        }
    }

    [Serializable]
    internal sealed class CountryFlagEntry
    {
        [LabelText("ISO Code")]
        [SerializeField] private string _countryId;

        [SerializeField, Required, PreviewField(64, ObjectFieldAlignment.Center)] private Sprite _flag;

        internal CountryId CountryId => new(_countryId);
        internal Sprite Flag => _flag;

        internal KeyValuePair<CountryId, Sprite> ToPair() => new(CountryId, Flag);
    }
}