using System;
using System.Linq;
using LL.Game.Currencies;
using LL.Game.ExperienceCards;
using LL.User.Core;
using LL.User.Core.ExperienceCards;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
using LL.User.Core.Wallet;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.User.Configuration
{
    [CreateAssetMenu(fileName = nameof(UserDefaultsConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class UserDefaultsConfig : ScriptableObject, IDefaultDataLoader<UserInitialData>
    {
        internal const string CreationPath = "LL/User/User Defaults Config";

        [BoxGroup("Identity")]
        [LabelText("Region Code")]
        [SerializeField] private string _regionCode;

        [BoxGroup("Identity")]
        [LabelText("User ID")]
        [SerializeField] private string _userId;

        [BoxGroup("Wallet")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CurrencyBalanceEntry[] _currencies;

        [BoxGroup("Progress")]
        [LabelText("Total Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _totalExperience;

        [BoxGroup("Experience Cards")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private ExperienceCardAmountEntry[] _experienceCards;

        UserInitialData IDataLoader<UserInitialData>.Load() => new
        (
            new UserIdentity(_regionCode, _userId),
            new WalletInitialData(_currencies?.Select(currency => currency?.ToData())),
            new ProgressInitialData(_totalExperience),
            new ExperienceCardsInitialData(_experienceCards?.Select(card => card?.ToData()))
        );
    }

    [Serializable]
    internal sealed class CurrencyBalanceEntry
    {
        [LabelText("Currency ID")] [SerializeField]
        private string _id;

        [LabelText("Amount")] [MinValue(0)] [SerializeField]
        private int _amount;

        internal CurrencyBalance ToData()
        {
            return new CurrencyBalance(
                new CurrencyId(_id),
                _amount);
        }
    }

    [Serializable]
    internal sealed class ExperienceCardAmountEntry
    {
        [LabelText("Card ID")] [SerializeField]
        private string _id;

        [LabelText("Amount")] [MinValue(0)] [SerializeField]
        private int _amount;

        internal ExperienceCardStack ToData()
        {
            return new ExperienceCardStack(
                new ExperienceCardId(_id),
                _amount);
        }
    }
}