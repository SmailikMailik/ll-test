using System;
using System.Linq;
using LL.Game.Cards;
using LL.Game.Items;
using LL.Identifiers;
using LL.User.Core;
using LL.User.Core.Amounts;
using LL.User.Core.Identity;
using LL.User.Core.Promotions;
using LL.User.Core.Progress;
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
        [HideLabel]
        [SerializeField] private UserIdentityDefaults _identity = new();

        [BoxGroup("Progress")]
        [HideLabel]
        [SerializeField] private UserProgressDefaults _progress = new();

        [BoxGroup("Items")]
        [ValidateInput(nameof(HasValidItemIds), "Item IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ItemAmountEntry[] _items;

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
                _identity.ToData(),
                new AmountsInitialData<ItemId>(_items?.Select(item => item.ToData())),
                _progress.ToData(),
                new PromotionOrderInitialData(default, 0L, false),
                new AmountsInitialData<CardId>(_cards?.Select(card => card.ToData())),
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
    [InlineProperty]
    internal sealed class UserIdentityDefaults
    {
        [LabelText("User ID")]
        [SerializeField] private string _userId;

        [LabelText("Region Code")]
        [SerializeField] private string _regionCode;

        internal UserIdentity ToData() => new(_userId, _regionCode);
    }

    [Serializable]
    [InlineProperty]
    internal sealed class UserProgressDefaults
    {
        [LabelText("Rank")]
        [MinValue(1)]
        [SerializeField] private int _rank = 1;

        [LabelText("Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _experience;

        internal ProgressInitialData ToData() => new(_rank, _experience);
    }

    [Serializable]
    internal abstract class AmountEntry<TId>
        where TId : struct, IIdentifier
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [LabelText("Amount")]
        [MinValue(0)]
        [SerializeField] private int _amount;

        internal TId Id => CreateId(_id);

        internal Amount<TId> ToData() => new(Id, _amount);

        protected abstract TId CreateId(string value);
    }

    [Serializable]
    internal sealed class ItemAmountEntry : AmountEntry<ItemId>
    {
        protected override ItemId CreateId(string value) => new(value);
    }

    [Serializable]
    internal sealed class CardAmountEntry : AmountEntry<CardId>
    {
        protected override CardId CreateId(string value) => new(value);
    }
}