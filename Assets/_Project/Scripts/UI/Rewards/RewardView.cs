using LL.Presentation.Icons;
using LL.Rewards;
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

        internal void UpdateView(IReward reward, RewardIconProvider iconProvider)
        {
            _iconImage.sprite = iconProvider.GetIcon(reward);
            _amountLabel.text = reward.Amount > 1 ? TextFormatter.Amount(reward.Amount) : string.Empty;
        }
    }
}