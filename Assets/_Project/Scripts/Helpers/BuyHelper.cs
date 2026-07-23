using System;
using System.Collections.Generic;
using UnityEngine;

namespace LL.Helpers
{
    public enum PriceType : byte
    {
        None = 0,
        Soft = 1,
        Hard = 2,
    }

    internal interface IPriceable
    {
        PriceType PriceType { get; }
        int PriceValue { get; }
        int PurchaseId { get; }
    }

    internal class CommonPrice : IPriceable
    {
        public PriceType PriceType { get; set; }
        public int PriceValue { get; set; }
        public int PurchaseId { get; set; }
    }

    internal static class BuyHelper
    {
        private static readonly Dictionary<PriceType, CurrencyType> _typesAssociations = new()
        {
            [PriceType.Soft] = CurrencyType.Soft
        };

        internal static void TryBuy(IPriceable priceable, Action successCallback)
        {
            switch (priceable.PriceType)
            {
                case PriceType.Soft:
                {
                    var currencyType = _typesAssociations[priceable.PriceType];
                    if (CurrenciesHelper.TrySpend(currencyType, priceable.PriceValue) is false)
                    {
                        //GlobalMessage.Instance.ShowForced(Localization.Get("ui.not-enough-soft"));
                        //WindowsController.Instance.Show(WindowType.Shop, new ShopParameters(ShopTabType.SoftPacks));
                        return;
                    }

                    successCallback?.Invoke();
                    break;
                }
                case PriceType.Hard:
                {
                    var currencyType = _typesAssociations[priceable.PriceType];
                    if (CurrenciesHelper.TrySpend(currencyType, priceable.PriceValue) is false)
                    {
                        //GlobalMessage.Instance.ShowForced(Localization.Get("ui.not-enough-hard"));
                        //WindowsController.Instance.Show(WindowType.Shop, new ShopParameters(ShopTabType.SoftPacks));
                        return;
                    }

                    successCallback?.Invoke();
                    break;
                }
                default:
                {
                    Debug.LogError($"[BuyHelper::Buy] Handler for type {priceable.PriceType} not defined!");
                    break;
                }
            }
        }

        internal static string GetPriceText(IPriceable priceable) => priceable.PriceType switch
        {
            PriceType.Soft => $"{priceable.PriceValue} {TextAtlasHelper.GetCurrencyIcon(CurrencyType.Soft)}",
            PriceType.Hard => $"{priceable.PriceValue} {TextAtlasHelper.GetCurrencyIcon(CurrencyType.Soft)}",
            _ => string.Empty
        };
    }
}