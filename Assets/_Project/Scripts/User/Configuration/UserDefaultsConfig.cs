using System.Linq;
using System;
using LL.Game.Items;
using LL.User.Snapshots;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(UserDefaultsConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class UserDefaultsConfig : ScriptableObject, IUserDefaultsProvider, IValidationSource
    {
        [BoxGroup("Identity")]
        [HideLabel]
        [SerializeField] private UserIdentityDefaults _identity = new();

        [BoxGroup("Progress")]
        [HideLabel]
        [SerializeField] private UserProgressDefaults _progress = new();

        [BoxGroup("Items")]
        [ValidateInput(nameof(HasValidItems), "User item defaults are invalid.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ItemAmountEntry[] _items;

        internal const string CreationPath = "LL/User/User Defaults Config";

        private static readonly IDataValidator<ItemAmountEntry[]> _itemsValidator =
            new UserItemsDefaultsValidator();

        private static readonly IDataValidator<UserDefaultsConfig> _validator =
            new UserDefaultsConfigValidator(_itemsValidator);

        internal UserIdentityDefaults Identity => _identity;
        internal UserProgressDefaults Progress => _progress;
        internal ItemAmountEntry[] Items => _items;

        UserSnapshot IUserDefaultsProvider.GetDefaultSnapshot()
        {
            ValidationRunner.EnsureValid(this);

            return new UserSnapshot(
                Identity.ToSnapshot(),
                new UserItemsSnapshot(Items.Select(item => item.ToItemAmount())),
                Progress.ToSnapshot(),
                UserPromotionOrderSnapshot.Empty,
                UserRewardClaimsSnapshot.Empty);
        }

        private static bool HasValidItems(ItemAmountEntry[] entries)
        {
            return ValidationRunner.IsValid(entries, _itemsValidator);
        }

        void IValidationSource.Validate(ValidationContext context)
        {
            _validator.Validate(this, context);
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

        internal string UserId => _userId;
        internal string RegionCode => _regionCode;

        internal UserIdentitySnapshot ToSnapshot() => new(UserId, RegionCode);
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

        internal int Rank => _rank;
        internal int Experience => _experience;

        internal UserProgressSnapshot ToSnapshot() => new(Rank, Experience);
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
        internal int Amount => _amount;

        internal ItemAmount ToItemAmount() => new(Id, Amount);
    }
}