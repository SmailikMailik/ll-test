using LL.Game.Cards.Services;
using LL.Game.Payments.Services;
using LL.Game.Ranks.Services;
using LL.Game.RankUp.Services;
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
            builder.Register<CardCollectionService>(Lifetime.Singleton).As<ICardCollectionService>();
            builder.Register<PaymentService>(Lifetime.Singleton).As<IPaymentService>();
            builder.Register<HeroExperienceService>(Lifetime.Singleton).As<IHeroExperienceService>();
            builder
                .Register<RankUpRequirementService>(Lifetime.Singleton)
                .As<IRankUpRequirementService>()
                .As<IRankUpQuestRequirementService>();
            builder.Register<CardExperienceService>(Lifetime.Singleton).As<ICardExperienceService>();
            builder.Register<RewardGrantService>(Lifetime.Singleton).As<IRewardGrantService>();
        }
    }
}