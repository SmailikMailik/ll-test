using System;
using System.Linq;
using LL.Game.Items;
using LL.Game.Identifiers;
using LL.User.Snapshots;
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

        internal const string CreationPath = "LL/User/User Defaults Config";

        UserSnapshot IUserDefaultsProvider.GetDefaultSnapshot()
        {
            IdentifierCollectionValidator.Validate(
                _items,
                entry => entry.Id,
                nameof(_items));
            return new UserSnapshot(
                _identity.ToSnapshot(),
                new UserItemsSnapshot(_items?.Select(item => item.ToItemAmount())),
                _progress.ToSnapshot(),
                new UserPromotionOrderSnapshot(default, 0L, false),
                new UserRewardClaimsSnapshot(null));
        }

        private static bool HasValidItemIds(ItemAmountEntry[] entries)
        {
            return IdentifierCollectionValidator.IsValid(entries, entry => entry.Id);
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

        internal UserIdentitySnapshot ToSnapshot() => new(_userId, _regionCode);
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

        internal UserProgressSnapshot ToSnapshot() => new(_rank, _experience);
    }

    [Serializable]
    internal sealed class ItemAmountEntry
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [LabelText("Amount")]
        [MinValue(0)]
        [SerializeField] private int _amount;

        internal ItemId Id => new(_id);

        internal ItemAmount ToItemAmount() => new(Id, _amount);
    }
}