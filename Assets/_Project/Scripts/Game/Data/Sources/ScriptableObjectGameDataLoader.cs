using System;
using LL.Game.Cards;
using LL.Game.Promotions;
using LL.Game.Quests;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Infrastructure.Loading;

namespace LL.Game.Data.Sources
{
    internal sealed class ScriptableObjectGameDataLoader : IDataLoader<GameDataSnapshot>
    {
        private readonly IDataLoader<RankCatalog> _ranksLoader;
        private readonly IDataLoader<CardCatalog> _cardsLoader;
        private readonly IDataLoader<QuestCatalog> _questsLoader;
        private readonly IDataLoader<RankPromotionCatalog> _rankPromotionsLoader;
        private readonly IDataLoader<RewardCatalog> _rewardsLoader;

        internal ScriptableObjectGameDataLoader(
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

        public GameDataSnapshot Load()
        {
            return new GameDataSnapshot(
                _ranksLoader.Load(),
                _cardsLoader.Load(),
                _questsLoader.Load(),
                _rankPromotionsLoader.Load(),
                _rewardsLoader.Load());
        }
    }
}