using LL.Game.Payments.Services;
using LL.Game.Rewards.Services;
using LL.Game.Upgrades.Services;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class GameServicesInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<PaymentService>(Lifetime.Singleton).As<IPaymentService>();
            builder.Register<CardExperienceService>(Lifetime.Singleton).As<ICardExperienceService>();
            builder.Register<RewardGrantService>(Lifetime.Singleton).As<IRewardGrantService>();
        }
    }
}