using System;
using System.Linq;
using LL.Game.Currencies;
using LL.Game.Cards;
using LL.Identifiers;
using LL.User.Core;
using LL.User.Core.Cards;
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
        [ValidateInput(nameof(HasValidCurrencyIds), "Currency IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CurrencyBalanceEntry[] _currencies;

        [BoxGroup("Progress")]
        [LabelText("Total Experience")]
        [SuffixLabel("XP", true)]
        [MinValue(0)]
        [SerializeField] private int _totalExperience;

        [BoxGroup("Cards")]
        [ValidateInput(nameof(HasValidCardIds), "Card IDs must be non-empty and unique.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private CardAmountEntry[] _cards;

        UserInitialData IDataLoader<UserInitialData>.Load()
        {
            IdentifierCatalogValidator.EnsureValidIds(
                _currencies,
                entry => entry.Id,
                "Default currency balances",
                nameof(_currencies));
            IdentifierCatalogValidator.EnsureValidIds(
                _cards,
                entry => entry.Id,
                "Default card amounts",
                nameof(_cards));

            return new UserInitialData(
                new UserIdentity(_regionCode, _userId),
                new WalletInitialData(_currencies?.Select(currency => currency.ToData())),
                new ProgressInitialData(_totalExperience),
                new CardsInitialData(_cards?.Select(card => card.ToData())));
        }

        private static bool HasValidCurrencyIds(CurrencyBalanceEntry[] entries)
        {
            return IdentifierCatalogValidator.HasValidIds(entries, entry => entry.Id);
        }

        private static bool HasValidCardIds(CardAmountEntry[] entries)
        {
            return IdentifierCatalogValidator.HasValidIds(entries, entry => entry.Id);
        }
    }

    [Serializable]
    internal sealed class CurrencyBalanceEntry
    {
        [LabelText("Currency ID")]
        [SerializeField] private string _id;

        [LabelText("Amount")]
        [MinValue(0)]
        [SerializeField] private int _amount;

        internal CurrencyId Id => new(_id);

        internal CurrencyBalance ToData() => new(Id, _amount);
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