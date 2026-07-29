using System;
using LL.User.State.Items;
using VContainer;

namespace LL.Game.Rewards.Services
{
    internal interface IRewardService
    {
        bool CanApply(IReward reward);
        bool TryApply(IReward reward);
    }

    internal sealed class RewardService : IRewardService
    {
        private readonly IUserItems _items;

        [Inject]
        internal RewardService(IUserItems items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public bool CanApply(IReward reward) => reward switch
        {
            ItemReward item => _items.CanAdd(item.ItemId, item.Amount),
            _ => false
        };

        public bool TryApply(IReward reward) => reward switch
        {
            ItemReward item => _items.TryAdd(item.ItemId, item.Amount),
            _ => false
        };
    }
}