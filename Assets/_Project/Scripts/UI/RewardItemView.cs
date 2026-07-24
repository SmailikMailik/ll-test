using LL.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LL.UI
{
    internal sealed class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Image _rewardImage;
        [SerializeField] private TMP_Text _rewardLabel;

        internal IReward Data { get; private set; }

        internal void UpdateView(IReward reward)
        {
            Data = reward;

            _rewardImage.sprite = RewardFormatter.GetIcon(reward);
            _rewardLabel.text = RewardFormatter.GetText(reward);
        }
    }
}