using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.User.Snapshots;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(UserDefaultsConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class UserDefaultsConfig : ScriptableObject, IUserDefaultsFactory, IValidationSource
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

        private static readonly IDataValidator<IReadOnlyList<ItemAmountEntry>> _itemsValidator = new UserItemsDefaultsValidator();

        private static readonly IDataValidator<UserDefaultsConfig> _validator = new UserDefaultsConfigValidator(_itemsValidator);

        internal UserIdentityDefaults Identity => _identity;
        internal UserProgressDefaults Progress => _progress;
        internal IReadOnlyList<ItemAmountEntry> Items => _items;

        UserSnapshot IUserDefaultsFactory.CreateSnapshot()
        {
            ValidationRunner.EnsureValid(this);

            return new UserSnapshot(
                Identity.ToSnapshot(),
                new UserItemsSnapshot(Items.Select(item => item.ToItemAmount())),
                Progress.ToSnapshot(),
                UserRankUpQuestSnapshot.Empty);
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
        [LabelText("Rank ID")]
        [SerializeField] private string _rankId;

        [LabelText("Experience")]
        [SuffixLabel("XP", true)]
        [SerializeField, MinValue(0)] private int _experience;

        internal RankId RankId => new(_rankId);
        internal int Experience => _experience;

        internal UserProgressSnapshot ToSnapshot() => new(RankId, Experience);
    }

    [Serializable]
    internal sealed class ItemAmountEntry
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [LabelText("Amount")]
        [SerializeField, MinValue(0)] private int _amount;

        internal ItemId Id => new(_id);
        internal int Amount => _amount;

        internal ItemAmount ToItemAmount() => new(Id, Amount);
    }
}