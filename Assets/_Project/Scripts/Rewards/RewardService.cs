using LL.User.Core.Progress;
using LL.User.Core.Wallet;

namespace LL.Rewards
{
    internal interface IRewardService
    {
        bool TryApply(IReward reward);
    }

    internal sealed class RewardService : IRewardService
    {
        private readonly IUserWallet _wallet;
        private readonly IUserProgress _progress;

        internal RewardService(IUserWallet wallet, IUserProgress progress)
        {
            _wallet = wallet;
            _progress = progress;
        }

        public bool TryApply(IReward reward) => reward switch
        {
            CurrencyReward currency => _wallet.TryAdd(currency.Currency, currency.Amount),
            ExperienceReward experience => _progress.TryAddExperience(experience.Amount),
            _ => false
        };
    }
}