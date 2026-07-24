using UnityEngine;

namespace LL.UI
{
    internal sealed class CurrenciesView : MonoBehaviour
    {
        [SerializeField] private CurrencyItemView _softItemView;

        /*private void Start()
        {
            UserData.Instance.SoftAmount.Changed += OnSoftAmountChanged;

            _softItemView.Button.Clicked += () =>
                WindowController.Instance.Show(new ShopParameters(ShopTabType.SoftPacks));

            OnSoftAmountChanged(UserData.Instance.SoftAmount.Value);
        }

        private void OnDestroy()
        {
            UserData.Instance.SoftAmount.Changed -= OnSoftAmountChanged;
        }*/

        private void OnSoftAmountChanged(int value) => _softItemView.Label.text = $"{value}";
    }
}