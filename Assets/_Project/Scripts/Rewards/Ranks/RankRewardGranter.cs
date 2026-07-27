using System;
using LL.User.Core.Progress;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.Rewards.Ranks
{
    internal sealed class RankRewardGranter : IInitializable, IDisposable
    {
        private readonly IUserProgress _progress;
        private readonly RankRewardCatalog _catalog;
        private readonly IRewardGrantService _rewardGrantService;

        private IDisposable _subscription;

        [Inject]
        internal RankRewardGranter(
            IUserProgress progress,
            RankRewardCatalog catalog,
            IRewardGrantService rewardGrantService)
        {
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
            _rewardGrantService = rewardGrantService ?? throw new ArgumentNullException(nameof(rewardGrantService));
        }

        public void Initialize()
        {
            _subscription = _progress.RankChanged.Subscribe(GrantReward);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }

        private void GrantReward(int rank)
        {
            if (_catalog.TryGetBundleId(rank, out var id))
                _rewardGrantService.TryGrant(id, out _);
        }
    }
}