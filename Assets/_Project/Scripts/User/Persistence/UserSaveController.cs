using System;
using System.Collections.Generic;
using System.Linq;
using LL.Game.Items;
using LL.Rewards.Models;
using LL.Saving;
using LL.User.Core.Items;
using LL.User.Core.Identity;
using LL.User.Core.Progress;
using LL.User.Core.Promotions;
using LL.User.Core.Rewards;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.User.Persistence
{
    internal sealed class UserSaveController : IInitializable, IDisposable
    {
        private readonly UserIdentity _identity;
        private readonly IUserItems _items;
        private readonly IUserProgress _progress;
        private readonly IUserPromotionOrder _promotionOrder;
        private readonly IUserRewardClaims _rewardClaims;
        private readonly ISaveService _saveService;
        private readonly List<IDisposable> _subscriptions = new();

        private readonly Dictionary<ItemId, int> _itemAmounts;
        private readonly HashSet<RewardBundleId> _claimedRewardIds;
        private ProgressInitialData _progressData;
        private PromotionOrderInitialData _promotionOrderData;

        [Inject]
        internal UserSaveController(
            UserIdentity identity,
            UserItemsInitialData itemAmounts,
            PromotionOrderInitialData promotionOrderData,
            IUserItems items,
            IUserProgress progress,
            IUserPromotionOrder promotionOrder,
            IUserRewardClaims rewardClaims,
            ISaveService saveService)
        {
            if (itemAmounts == null)
                throw new ArgumentNullException(nameof(itemAmounts));

            _identity = identity ?? throw new ArgumentNullException(nameof(identity));
            _itemAmounts = itemAmounts.Amounts.ToDictionary(item => item.Id, item => item.Amount);
            _promotionOrderData = promotionOrderData ?? throw new ArgumentNullException(nameof(promotionOrderData));
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _progressData = new ProgressInitialData(progress.Rank, progress.Experience);
            _promotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            _rewardClaims = rewardClaims ?? throw new ArgumentNullException(nameof(rewardClaims));
            _claimedRewardIds = new HashSet<RewardBundleId>(rewardClaims.ClaimedIds);
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));
        }

        public void Initialize()
        {
            ObserveItems();

            _subscriptions.Add(_progress.RankChanged.Subscribe(_ => UpdateProgressAndSave()));
            _subscriptions.Add(_progress.ExperienceChanged.Subscribe(_ => UpdateProgressAndSave()));
            _subscriptions.Add(_promotionOrder.Changed.Subscribe(_ => UpdatePromotionOrderAndSave()));
            _subscriptions.Add(_rewardClaims.RewardClaimed.Subscribe(AddClaimAndSave));
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
        }

        private void ObserveItems()
        {
            foreach (var id in _itemAmounts.Keys.ToArray())
            {
                var itemId = id;
                _subscriptions.Add(_items
                    .ObserveAmount(itemId)
                    .Subscribe(value => UpdateAmountAndSave(itemId, value)));
            }
        }

        private void UpdateAmountAndSave(ItemId id, int value)
        {
            if (_itemAmounts[id] == value)
                return;

            _itemAmounts[id] = value;
            Save();
        }

        private void AddClaimAndSave(RewardBundleId id)
        {
            if (_claimedRewardIds.Add(id))
                Save();
        }

        private void UpdatePromotionOrderAndSave()
        {
            var data = new PromotionOrderInitialData(
                _promotionOrder.RequirementId,
                _promotionOrder.DeadlineUnixMilliseconds,
                _promotionOrder.IsCompleted);

            if (_promotionOrderData.RequirementId.Equals(data.RequirementId) &&
                _promotionOrderData.DeadlineUnixMilliseconds == data.DeadlineUnixMilliseconds &&
                _promotionOrderData.IsCompleted == data.IsCompleted)
            {
                return;
            }

            _promotionOrderData = data;
            Save();
        }

        private void UpdateProgressAndSave()
        {
            var data = new ProgressInitialData(
                _progress.Rank,
                _progress.Experience);

            if (_progressData.Rank == data.Rank &&
                _progressData.Experience == data.Experience)
            {
                return;
            }

            _progressData = data;
            Save();
        }

        private void Save()
        {
            var data = UserSaveDataMapper.ToSaveData(
                _identity,
                _progressData,
                _itemAmounts.Select(pair => new ItemAmount(pair.Key, pair.Value)),
                _promotionOrderData,
                _claimedRewardIds);

            _saveService.TrySave(UserInitialDataLoader.SaveKey, data);
        }
    }
}