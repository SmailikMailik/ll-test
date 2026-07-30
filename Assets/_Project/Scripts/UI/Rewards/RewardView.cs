using LL.UI.Typography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI.Rewards
{
    [DisallowMultipleComponent]
    internal sealed class RewardView : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;

        [SerializeField] private TMP_Text _amountLabel;

        internal void UpdateView(Sprite icon, int amount)
        {
            _iconImage.sprite = icon;
            _amountLabel.text = TextFormatter.Amount(amount);
        }
    }
}