using System;
using LL.Composition.Factories;
using LL.Game.Data;
using LL.Game.Data.Configuration;
using LL.Game.Cards;
using LL.Game.RankUp;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;
using VContainer;
using VContainer.Unity;

namespace LL.Composition.Installers
{
    internal sealed class GameDataInstaller : IInstaller
    {
        private readonly GameDataManifestConfig _manifest;

        internal GameDataInstaller(GameDataManifestConfig manifest)
        {
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
        }

        public void Install(IContainerBuilder builder)
        {
            var loader = GameDataLoaderFactory.CreateFromScriptableObjects(_manifest);

            builder.RegisterLoadedData(loader);
            builder.RegisterSnapshotPart<GameDataSnapshot, RankCatalog>(snapshot => snapshot.Ranks);
            builder.RegisterSnapshotPart<GameDataSnapshot, CardCatalog>(snapshot => snapshot.Cards);
            builder.RegisterSnapshotPart<GameDataSnapshot, QuestCatalog>(snapshot => snapshot.Quests);
            builder.RegisterSnapshotPart<GameDataSnapshot, RankUpCatalog>(snapshot => snapshot.RankUps);
            builder.RegisterSnapshotPart<GameDataSnapshot, RewardCatalog>(snapshot => snapshot.Rewards);
        }
    }
}