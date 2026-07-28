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
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amountLabel;

        internal IReward Data { get; private set; }

        internal void UpdateView(IReward reward, RewardIconProvider iconProvider)
        {
            Data = reward;

            _icon.sprite = iconProvider.GetIcon(reward);
            _amountLabel.text = TextFormatter.Amount(reward.Amount);
        }
    }
}