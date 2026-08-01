using System;
using LL.Game.Ranks;
using LL.Infrastructure.Compilation;
using LL.Infrastructure.Loading;
using LL.User.Defaults;
using LL.User.Defaults.Declarations;
using LL.User.Defaults.Sources;
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
        private readonly IDataSource<UserDefaultsDeclaration> _defaultsSource;
        private readonly IUserSaveRepository _saveRepository;

        internal UserInstaller(
            IDataSource<UserDefaultsDeclaration> defaultsSource,
            IUserSaveRepository saveRepository)
        {
            _defaultsSource = defaultsSource ?? throw new ArgumentNullException(nameof(defaultsSource));
            _saveRepository = saveRepository ?? throw new ArgumentNullException(nameof(saveRepository));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_defaultsSource);
            builder.RegisterInstance(_saveRepository);
            builder.RegisterInstance(TimeProvider.System);
            builder
                .Register<UserDefaultsCompiler>(Lifetime.Singleton)
                .As<IDataCompiler<UserDefaultsDeclaration, UserDefaultsSnapshot>>();
            builder
                .Register(
                    resolver => new CompiledDataLoader<UserDefaultsDeclaration, UserDefaultsSnapshot>(
                        resolver.Resolve<IDataSource<UserDefaultsDeclaration>>(),
                        resolver.Resolve<IDataCompiler<UserDefaultsDeclaration, UserDefaultsSnapshot>>()),
                    Lifetime.Singleton)
                .As<IDataLoader<UserDefaultsSnapshot>>();
            builder.RegisterLoadedData<UserDefaultsSnapshot>();
            builder.Register<UserSnapshotReconciler>(Lifetime.Singleton);
            builder.Register<UserSessionLoader>(Lifetime.Singleton).As<IDataLoader<UserSnapshot>>();
            builder.RegisterLoadedData<UserSnapshot>();
            builder.RegisterSnapshotPart<UserSnapshot, UserIdentitySnapshot>(snapshot => snapshot.Identity);
            builder.RegisterSnapshotPart<UserSnapshot, UserHeroSelectionSnapshot>(snapshot => snapshot.HeroSelection);
            builder.RegisterSnapshotPart<UserSnapshot, UserProgressSnapshot>(snapshot => snapshot.Progress);
            builder.RegisterSnapshotPart<UserSnapshot, UserRankUpQuestSnapshot>(snapshot => snapshot.RankUpQuest);
            builder.RegisterSnapshotPart<UserSnapshot, UserItemsSnapshot>(snapshot => snapshot.Items);

            builder
                .Register<UserItems>(Lifetime.Singleton)
                .As<IUserItems>()
                .As<IUserItemsCommands>();
            builder
                .Register<UserRankUpQuest>(Lifetime.Singleton)
                .As<IUserRankUpQuest>()
                .As<IUserRankUpQuestCommands>();
            builder.Register<RankProgression>(Lifetime.Singleton).As<IRankProgression>();
            builder
                .Register<UserProgress>(Lifetime.Singleton)
                .As<IUserProgress>()
                .As<IUserProgressCommands>();
            builder.RegisterEntryPoint<UserState>().AsSelf().As<IUserStateChangeBatch>();
            builder.RegisterEntryPoint<UserSaveCoordinator>();
        }
    }
}