using System;
using LL.Composition.Factories;
using LL.Game.Ranks;
using LL.Infrastructure.Loading;
using LL.User.Configuration;
using LL.User.Persistence;
using LL.User.Snapshots;
using LL.User.State;
using LL.User.State.Items;
using LL.User.State.Progress;
using LL.User.State.RankUp;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class UserInstaller : IInstaller
    {
        private readonly IUserDefaultsFactory _defaultsFactory;

        internal UserInstaller(IUserDefaultsFactory defaultsFactory)
        {
            _defaultsFactory = defaultsFactory ?? throw new ArgumentNullException(nameof(defaultsFactory));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_defaultsFactory);
            builder.RegisterInstance<IUserSaveRepository>(UserSaveRepositoryFactory.CreateJsonFile());
            builder.Register<UserSessionLoader>(Lifetime.Singleton).As<IDataLoader<UserSnapshot>>();
            builder.RegisterLoadedData<UserSnapshot>();
            builder.RegisterSnapshotPart<UserSnapshot, UserIdentitySnapshot>(snapshot => snapshot.Identity);
            builder.RegisterSnapshotPart<UserSnapshot, UserItemsSnapshot>(snapshot => snapshot.Items);
            builder.RegisterSnapshotPart<UserSnapshot, UserProgressSnapshot>(snapshot => snapshot.Progress);
            builder.RegisterSnapshotPart<UserSnapshot, UserRankUpQuestSnapshot>(snapshot => snapshot.RankUpQuest);

            builder.Register<UserItems>(Lifetime.Singleton).As<IUserItems>();
            builder.Register<UserRankUpQuest>(Lifetime.Singleton).As<IUserRankUpQuest>();
            builder.Register<RankProgression>(Lifetime.Singleton).As<IRankProgression>();
            builder.Register<UserProgress>(Lifetime.Singleton).As<IUserProgress>();
            builder.RegisterEntryPoint<UserState>().AsSelf();
            builder.RegisterEntryPoint<UserSaveCoordinator>();
        }
    }
}