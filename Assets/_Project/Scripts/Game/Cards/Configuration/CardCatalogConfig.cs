using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Cards.Configuration
{
    [CreateAssetMenu(fileName = nameof(CardCatalogConfig), menuName = CreationPath)]
    internal sealed class CardCatalogConfig :
        ScriptableObject,
        IDataLoader<CardCatalog>,
        IValidationSource
    {
        [ValidateInput(nameof(HasValidCards), "Card data is invalid.")]
        [SerializeField] private CardEntry[] _cards;

        internal const string CreationPath = "LL/Game Data/Card Catalog";

        private static readonly IDataValidator<CardEntry[]> _validator =
            new CardCatalogConfigValidator();

        internal IReadOnlyList<CardEntry> Cards => _cards;

        public CardCatalog Load()
        {
            ValidationRunner.EnsureValid(this, nameof(_cards));

            return new CardCatalog(_cards.Select(entry => entry.ToCard()));
        }

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
        [SerializeField, MinValue(1)] private int _experienceAmount;

        internal ItemId Id => new(_id);
        internal int ExperienceAmount => _experienceAmount;

        internal ICard ToCard() => new Card(Id, ExperienceAmount);
    }
}