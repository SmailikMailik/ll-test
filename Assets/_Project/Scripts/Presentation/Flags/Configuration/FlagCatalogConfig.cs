using System;
using System.Collections.Generic;
using LL.Game.Flags;
using LL.Presentation.Inspector;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace LL.Presentation.Flags.Configuration
{
    [CreateAssetMenu(fileName = nameof(FlagCatalogConfig), menuName = CreationPath)]
    [MovedFrom(true, "LL.Presentation.Countries.Configuration", null, "CountryFlagCatalogConfig")]
    internal sealed class FlagCatalogConfig : ScriptableObject, IValidationSource
    {
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [ValidateInput(nameof(HasValidFlags), "Flag data is invalid.")]
        [SerializeField] private FlagEntry[] _flags;

        internal const string CreationPath = "LL/Presentation/Flag Catalog";

        private static readonly IDataValidator<FlagEntry[]> _validator = new FlagCatalogConfigValidator();

        internal IReadOnlyList<FlagEntry> Flags => _flags;

        private static bool HasValidFlags(FlagEntry[] flags) => ValidationRunner.IsValid(flags, _validator);

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_flags, context);
        }
    }

    [Serializable]
    internal sealed class FlagEntry
    {
        [LabelText("Flag ID")]
        [SerializeField] private string _flagId;

        [SpritePreview]
        [SerializeField, Required] private Sprite _flag;

        internal FlagId Id => new(_flagId);
        internal Sprite Flag => _flag;

        internal KeyValuePair<FlagId, Sprite> ToPair() => new(Id, Flag);
    }
}