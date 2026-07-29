using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.UI.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LL.UI.Windows.Views.Reward
{
    [DisallowMultipleComponent]
    internal sealed class RewardWindow : Window<RewardWindowParameters>, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _rankLabel;
        [SerializeField] private RewardContainerView _rewardContainer;
        [SerializeField] private RewardWindowAnimation _animation;

        protected override void OnShow()
        {
            _rankLabel.text = Parameters.Rank.ToString();
            _rewardContainer.SetItems(Parameters.Items);
            _animation.Play();
        }

        protected override void OnHide()
        {
            _animation.ResetView();
        }

        public void OnPointerClick(PointerEventData _) => TryClose();
    }

    internal sealed class RewardWindowParameters : IWindowParameters
    {
        internal int Rank { get; }
        internal IReadOnlyList<ItemAmount> Items { get; }

        internal RewardWindowParameters(int rank, IReadOnlyList<ItemAmount> items)
        {
            Rank = rank;
            Items = Array.AsReadOnly(items?.ToArray() ?? Array.Empty<ItemAmount>());
        }
    }
}