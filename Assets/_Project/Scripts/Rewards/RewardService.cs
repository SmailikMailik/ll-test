using LL.User.Core.Progress;
using LL.User.Core.Wallet;
using VContainer;

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

        [Inject]
        internal RewardService(IUserWallet wallet, IUserProgress progress)
        {
            _wallet = wallet;
            _progress = progress;
        }

        public bool TryApply(IReward reward) => reward switch
        {
            CurrencyReward currency => _wallet.TryAdd(currency.CurrencyId, currency.Amount),
            ExperienceReward experience => _progress.TryAddExperience(experience.Amount),
            _ => false
        };
    }
}