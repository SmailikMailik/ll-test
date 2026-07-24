using LL.User;

namespace LL.Rewards
{
    internal interface IReward
    {
        int Amount { get; }
    }

    internal sealed class CurrencyReward : IReward
    {
        internal CurrencyType Currency { get; }
        public int Amount { get; }

        internal CurrencyReward(CurrencyType currency, int amount)
        {
            Currency = currency;
            Amount = amount;
        }
    }

    internal sealed class ExperienceReward : IReward
    {
        public int Amount { get; }

        internal ExperienceReward(int amount)
        {
            Amount = amount;
        }
    }
}