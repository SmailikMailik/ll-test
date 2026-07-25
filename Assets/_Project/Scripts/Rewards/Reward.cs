using LL.Game.Currencies;

namespace LL.Rewards
{
    internal interface IReward
    {
        int Amount { get; }
    }

    internal sealed class CurrencyReward : IReward
    {
        internal CurrencyId CurrencyId { get; }
        public int Amount { get; }

        internal CurrencyReward(CurrencyId currencyId, int amount)
        {
            CurrencyId = currencyId;
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