using System;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Identifiers;
using LL.User.Core;
using LL.User.Core.Cards;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
using LL.User.Core.Items;
using LL.User.Core.Rewards;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(UserDefaultsConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class UserDefaultsConfig : ScriptableObject, IUserDefaultsProvider
    {
        [BoxGroup("Identity")]
        [LabelText("Region Code")]
        [SerializeField] private string _regionCode;

        [BoxGroup("Identity")]
        [LabelText("User ID")]
        [SerializeField] private string _userId;

        [BoxGroup("Items")]
        [ValidateInput(nameof(HasValidItemIds), "Item IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ItemAmountEntry[] _items;

        [BoxGroup("Progress")]
        [LabelText("Rank")]
        [MinValue(1)]
        [SerializeField] private int _rank = 1;

        [BoxGroup("Progress")]
        [LabelText("Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _experience;

        [BoxGroup("Cards")]
        [ValidateInput(nameof(HasValidCardIds), "Card IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CardAmountEntry[] _cards;

        internal const string CreationPath = "LL/User/User Defaults Config";

        UserInitialData IUserDefaultsProvider.GetDefaults()
        {
            IdentifierCatalogValidator.EnsureValidIds(
                _items,
                entry => entry.Id,
                "Default item amounts",
                nameof(_items));
            IdentifierCatalogValidator.EnsureValidIds(
                _cards,
                entry => entry.Id,
                "Default card amounts",
                nameof(_cards));
            return new UserInitialData(
                new UserIdentity(_regionCode, _userId),
                new ItemsInitialData(_items?.Select(item => item.ToData())),
                new ProgressInitialData(_rank, _experience),
                new CardsInitialData(_cards?.Select(card => card.ToData())),
                new RewardClaimsInitialData(null));
        }

        private static bool HasValidItemIds(ItemAmountEntry[] entries)
        {
            return IdentifierCatalogValidator.HasValidIds(entries, entry => entry.Id);
        }

        private static bool HasValidCardIds(CardAmountEntry[] entries)
        {
            return IdentifierCatalogValidator.HasValidIds(entries, entry => entry.Id);
        }
    }

    [Serializable]
    internal sealed class ItemAmountEntry
    {
        [LabelText("Item ID")]
        [SerializeField] private string _id;

        [LabelText("Amount")]
        [MinValue(0)]
        [SerializeField] private int _amount;

        internal ItemId Id => new(_id);

        internal ItemAmount ToData() => new(Id, _amount);
    }

    [Serializable]
    internal sealed class CardAmountEntry
    {
        [LabelText("Card ID")]
        [SerializeField] private string _id;

        [LabelText("Amount")]
        [MinValue(0)]
        [SerializeField] private int _amount;

        internal CardId Id => new(_id);

        internal CardStack ToData() => new(Id, _amount);
    }
}