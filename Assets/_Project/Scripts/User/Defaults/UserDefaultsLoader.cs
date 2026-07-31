using System;
using LL.Game.Cards;
using LL.Game.RankUp;
using LL.Game.Ranks;
using LL.Game.Rewards;
using LL.Infrastructure.Loading;
using LL.User.Defaults.Sources;
using VContainer;

namespace LL.User.Defaults
{
    internal sealed class UserDefaultsLoader : IDataLoader<UserDefaultsSnapshot>
    {
        private readonly IUserDefaultsSource _source;
        private readonly UserDefaultsCompiler _compiler;
        private readonly RankCatalog _ranks;
        private readonly CardCatalog _cards;
        private readonly RankUpCatalog _rankUps;
        private readonly RewardCatalog _rewards;

        [Inject]
        internal UserDefaultsLoader(
            IUserDefaultsSource source,
            UserDefaultsCompiler compiler,
            RankCatalog ranks,
            CardCatalog cards,
            RankUpCatalog rankUps,
            RewardCatalog rewards)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _compiler = compiler ?? throw new ArgumentNullException(nameof(compiler));
            _ranks = ranks ?? throw new ArgumentNullException(nameof(ranks));
            _cards = cards ?? throw new ArgumentNullException(nameof(cards));
            _rankUps = rankUps ?? throw new ArgumentNullException(nameof(rankUps));
            _rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
        }

        public UserDefaultsSnapshot Load()
        {
            return _compiler.Compile(_source.Read(), _ranks, _cards, _rankUps, _rewards);
        }
    }
}