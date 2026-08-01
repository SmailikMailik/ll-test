using System;
using System.Collections.Generic;
using LL.Game.Heroes;
using LL.Game.Items;
using LL.Game.Ranks;
using LL.Validation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(UserDefaultsConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class UserDefaultsConfig : ScriptableObject, IValidationSource
    {
        [BoxGroup("Identity")]
        [HideLabel]
        [SerializeField] private UserIdentityDefaults _identity = new();

        [BoxGroup("Hero Selection")]
        [HideLabel]
        [SerializeField] private UserHeroSelectionDefaults _heroSelection = new();

        [BoxGroup("Progress")]
        [HideLabel]
        [SerializeField] private UserProgressDefaults _progress = new();

        [BoxGroup("Items")]
        [ValidateInput(nameof(HasValidItems), "User item defaults are invalid.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private UserItemDefaultEntry[] _items;

        internal const string CreationPath = "LL/User/User Defaults Config";

        private static readonly IDataValidator<IReadOnlyList<UserItemDefaultEntry>> _itemsValidator = new UserItemDefaultsValidator();
        private static readonly IDataValidator<UserDefaultsConfig> _validator = new UserDefaultsConfigValidator(_itemsValidator);

        internal UserIdentityDefaults Identity => _identity;
        internal UserHeroSelectionDefaults HeroSelection => _heroSelection;
        internal UserProgressDefaults Progress => _progress;
        internal IReadOnlyList<UserItemDefaultEntry> Items => _items;

        private static bool HasValidItems(UserItemDefaultEntry[] entries)
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
    internal sealed class UserHeroSelectionDefaults
    {
        [LabelText("Hero ID")]
        [SerializeField] private string _heroId;

        internal HeroId HeroId => new(_heroId);
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
    }

    [Serializable]
    [InlineProperty]
    internal sealed class UserProgressDefaults
    {
        [LabelText("Rank ID")]
        [SerializeField] private string _rankId;

        [LabelText("Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _experience;

        internal RankId RankId => new(_rankId);
        internal int Experience => _experience;
    }

    [Serializable]
    internal sealed class UserItemDefaultEntry
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [LabelText("Amount")]
        [MinValue(0)]
        [SerializeField] private int _amount;

        internal ItemId Id => new(_id);
        internal int Amount => _amount;
    }
}