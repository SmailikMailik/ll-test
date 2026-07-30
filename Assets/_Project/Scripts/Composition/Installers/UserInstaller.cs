using System;
using LL.Composition.Factories;
using LL.Game.Ranks;
using LL.Infrastructure.Loading;
using LL.Infrastructure.Saving;
using LL.User.Configuration;
using LL.User.Persistence;
using LL.User.Snapshots;
using LL.User.State.Items;
using LL.User.State.Progress;
using LL.User.State.Promotions;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class UserInstaller : IInstaller
    {
        private readonly IUserDefaultsProvider _defaultsProvider;

        internal UserInstaller(IUserDefaultsProvider defaultsProvider)
        {
            _defaultsProvider =
                defaultsProvider ?? throw new ArgumentNullException(nameof(defaultsProvider));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_defaultsProvider);
            builder.RegisterInstance<ISaveService>(UserSaveServiceFactory.CreateJsonFile());
            builder.Register<UserSnapshotLoader>(Lifetime.Singleton).As<IDataLoader<UserSnapshot>>();
            builder.RegisterLoadedData<UserSnapshot>();
            builder.RegisterSnapshotPart<UserSnapshot, UserIdentitySnapshot>(snapshot => snapshot.Identity);
            builder.RegisterSnapshotPart<UserSnapshot, UserItemsSnapshot>(snapshot => snapshot.Items);
            builder.RegisterSnapshotPart<UserSnapshot, UserProgressSnapshot>(snapshot => snapshot.Progress);
            builder.RegisterSnapshotPart<UserSnapshot, UserPromotionQuestSnapshot>(
                snapshot => snapshot.PromotionQuest);

            builder.Register<UserItems>(Lifetime.Singleton).As<IUserItems>();
            builder.Register<UserPromotionQuest>(Lifetime.Singleton).As<IUserPromotionQuest>();
            builder.Register<RankProgression>(Lifetime.Singleton).As<IRankProgression>();
            builder.Register<UserProgress>(Lifetime.Singleton).As<IUserProgress>();
            builder.RegisterEntryPoint<UserSaveController>();
        }
    }
}