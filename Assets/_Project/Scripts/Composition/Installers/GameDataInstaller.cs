using System;
using LL.Composition.Factories;
using LL.Game.Cards;
using LL.Game.Data;
using LL.Game.Promotions;
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
        private readonly IDataLoader<RankCatalog> _ranksLoader;
        private readonly IDataLoader<CardCatalog> _cardsLoader;
        private readonly IDataLoader<QuestCatalog> _questsLoader;
        private readonly IDataLoader<RankPromotionCatalog> _rankPromotionsLoader;
        private readonly IDataLoader<RewardCatalog> _rewardsLoader;

        internal GameDataInstaller(
            IDataLoader<RankCatalog> ranksLoader,
            IDataLoader<CardCatalog> cardsLoader,
            IDataLoader<QuestCatalog> questsLoader,
            IDataLoader<RankPromotionCatalog> rankPromotionsLoader,
            IDataLoader<RewardCatalog> rewardsLoader)
        {
            _ranksLoader = ranksLoader ?? throw new ArgumentNullException(nameof(ranksLoader));
            _cardsLoader = cardsLoader ?? throw new ArgumentNullException(nameof(cardsLoader));
            _questsLoader = questsLoader ?? throw new ArgumentNullException(nameof(questsLoader));
            _rankPromotionsLoader =
                rankPromotionsLoader ?? throw new ArgumentNullException(nameof(rankPromotionsLoader));
            _rewardsLoader = rewardsLoader ?? throw new ArgumentNullException(nameof(rewardsLoader));
        }

        public void Install(IContainerBuilder builder)
        {
            var loader = GameDataLoaderFactory.CreateFromScriptableObjects(
                _ranksLoader,
                _cardsLoader,
                _questsLoader,
                _rankPromotionsLoader,
                _rewardsLoader);

            builder.RegisterLoadedData(loader);
            builder.RegisterSnapshotPart<GameDataSnapshot, RankCatalog>(snapshot => snapshot.Ranks);
            builder.RegisterSnapshotPart<GameDataSnapshot, CardCatalog>(snapshot => snapshot.Cards);
            builder.RegisterSnapshotPart<GameDataSnapshot, QuestCatalog>(snapshot => snapshot.Quests);
            builder.RegisterSnapshotPart<GameDataSnapshot, RankPromotionCatalog>(
                snapshot => snapshot.RankPromotions);
            builder.RegisterSnapshotPart<GameDataSnapshot, RewardCatalog>(snapshot => snapshot.Rewards);
        }
    }
}