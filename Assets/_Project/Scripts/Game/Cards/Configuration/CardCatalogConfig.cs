using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Cards.Configuration
{
    [CreateAssetMenu(fileName = nameof(CardCatalogConfig), menuName = CreationPath)]
    internal sealed class CardCatalogConfig : ScriptableObject, IValidationSource
    {
        [ValidateInput(nameof(HasValidCards), "Card data is invalid.")]
        [SerializeField] private CardEntry[] _cards;

        internal const string CreationPath = "LL/Game Data/Card Catalog";

        private static readonly IDataValidator<CardEntry[]> _validator = new CardCatalogConfigValidator();

        internal IReadOnlyList<CardEntry> Cards => _cards;

        private static bool HasValidCards(CardEntry[] cards)
        {
            return ValidationRunner.IsValid(cards, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_cards, context);
        }
    }

    [Serializable]
    internal sealed class CardEntry
    {
        [SerializeField] private string _id;
        [MinValue(1)]
        [SerializeField] private int _experienceAmount;

        internal ItemId Id => new(_id);
        internal int ExperienceAmount => _experienceAmount;
    }
}