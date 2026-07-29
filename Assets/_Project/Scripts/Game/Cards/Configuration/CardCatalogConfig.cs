using System;
using System.Linq;
using LL.Game.Cards;
using LL.Identifiers;
using LL.Loading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Cards.Configuration
{
    [CreateAssetMenu(fileName = nameof(CardCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class CardCatalogConfig : ScriptableObject, IDataLoader<CardCatalog>
    {
        internal const string CreationPath = "LL/Game Data/Card Catalog";

        [ValidateInput(nameof(HasValidCardIds), "Card IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CardDefinitionEntry[] _cards;

        public CardCatalog Load() => new
        (
            _cards?.Select(card => card?.ToCard())
        );

        private void OnValidate()
        {
            if (_cards == null)
                return;

            for (var index = 0; index < _cards.Length; index++)
            {
                var entry = _cards[index] ?? new CardDefinitionEntry(string.Empty, 1);
                entry.Normalize();
                _cards[index] = entry;
            }
        }

        private static bool HasValidCardIds(CardDefinitionEntry[] cards)
        {
            return IdentifierCatalogValidator.HasValidIds(cards, card => new CardId(card.Id));
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

        internal string Id => _id;

        internal CardDefinitionEntry(string id, int experienceAmount)
        {
            _id = id;
            _experienceAmount = experienceAmount;
        }

        internal ICard ToCard() => new Card
        (
            new CardId(_id),
            _experienceAmount
        );

        internal void Normalize()
        {
            _experienceAmount = Math.Max(1, _experienceAmount);
        }
    }
}