using System;
using LL.Game.Data;
using LL.Game.Cards;
using LL.Game.RankUp;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Infrastructure.Loading;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class GameDataInstaller : IInstaller
    {
        private readonly IDataLoader<GameDataSnapshot> _loader;

        internal GameDataInstaller(IDataLoader<GameDataSnapshot> loader)
        {
            _loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        public void Install(IContainerBuilder builder)
        {
            builder.RegisterLoadedData(_loader);
            builder.RegisterSnapshotPart<GameDataSnapshot, RankCatalog>(snapshot => snapshot.Ranks);
            builder.RegisterSnapshotPart<GameDataSnapshot, CardCatalog>(snapshot => snapshot.Cards);
            builder.RegisterSnapshotPart<GameDataSnapshot, QuestCatalog>(snapshot => snapshot.Quests);
            builder.RegisterSnapshotPart<GameDataSnapshot, RankUpCatalog>(snapshot => snapshot.RankUps);
            builder.RegisterSnapshotPart<GameDataSnapshot, RewardCatalog>(snapshot => snapshot.Rewards);
        }
    }
}