using System;
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
        private readonly IUserSaveRepository _saveRepository;

        internal UserInstaller(
            IUserDefaultsFactory defaultsFactory,
            IUserSaveRepository saveRepository)
        {
            _defaultsFactory = defaultsFactory ?? throw new ArgumentNullException(nameof(defaultsFactory));
            _saveRepository = saveRepository ?? throw new ArgumentNullException(nameof(saveRepository));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_defaultsFactory);
            builder.RegisterInstance(_saveRepository);
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