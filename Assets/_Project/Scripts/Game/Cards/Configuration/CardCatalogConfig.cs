using System;
using System.Linq;
using LL.Game.Items;
using LL.Infrastructure.Loading;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Cards.Configuration
{
    [CreateAssetMenu(fileName = nameof(CardCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class CardCatalogConfig : ScriptableObject, IDataLoader<CardCatalog>, IValidationSource
    {
        [ValidateInput(nameof(HasValidCards), "Card data is invalid.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CardDefinitionEntry[] _cards;

        internal const string CreationPath = "LL/Game Data/Card Catalog";

        private static readonly IDataValidator<CardDefinitionEntry[]> _validator =
            new CardCatalogConfigValidator();

        internal CardDefinitionEntry[] Cards => _cards;

        public CardCatalog Load()
        {
            ValidationRunner.EnsureValid(this, nameof(_cards));

            return new CardCatalog(_cards?.Select(card => card.ToCard()));
        }

        private static bool HasValidCards(CardDefinitionEntry[] cards)
        {
            return ValidationRunner.IsValid(cards, _validator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(_cards, context);
        }
    }

    [Serializable]
    internal sealed class CardDefinitionEntry
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [LabelText("Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(1)]
        [SerializeField] private int _experienceAmount;

        internal ItemId Id => new(_id);
        internal int ExperienceAmount => _experienceAmount;

        internal ICard ToCard() => new Card
        (
            Id,
            ExperienceAmount
        );
    }
}