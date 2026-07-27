using System;
using System.Linq;
using LL.Game.Currencies;
using LL.Game.Purchases;
using LL.Identifiers;
using LL.Loading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LL.Game.Configuration
{
    [CreateAssetMenu(fileName = nameof(PurchaseCatalogConfig), menuName = CreationPath)]
    [HideMonoScript]
    internal sealed class PurchaseCatalogConfig : ScriptableObject, IDataLoader<PurchaseCatalog>
    {
        [ValidateInput(nameof(HasValidPurchases), "Purchase IDs must be unique and currency IDs must be non-empty.")]
        [TableList(AlwaysExpanded = true, DrawScrollView = false)]
        [SerializeField] private PurchaseEntry[] _purchases;

        internal const string CreationPath = "LL/Game Data/Purchase Catalog";

        public PurchaseCatalog Load() => new(_purchases?.Select(entry => entry?.ToPurchase()));

        private void OnValidate()
        {
            if (_purchases == null)
                return;

            for (var index = 0; index < _purchases.Length; index++)
            {
                var entry = _purchases[index] ?? new PurchaseEntry();
                entry.Normalize();
                _purchases[index] = entry;
            }
        }

        private static bool HasValidPurchases(PurchaseEntry[] purchases)
        {
            if (purchases == null)
                return true;

            return IdentifierCatalogValidator.HasValidIds(
                       purchases,
                       purchase => purchase.Id) &&
                   purchases.All(purchase => purchase.CurrencyId.IsEmpty is false);
        }
    }

    [Serializable]
    internal sealed class PurchaseEntry
    {
        [LabelText("ID")]
        [SerializeField] private string _id;

        [LabelText("Currency ID")]
        [SerializeField] private string _currencyId;

        [MinValue(1)]
        [SerializeField] private int _price;

        internal PurchaseId Id => new(_id);
        internal CurrencyId CurrencyId => new(_currencyId);

        internal IPurchase ToPurchase() => new Purchase(Id, CurrencyId, _price);

        internal void Normalize()
        {
            _price = Math.Max(1, _price);
        }
    }
}