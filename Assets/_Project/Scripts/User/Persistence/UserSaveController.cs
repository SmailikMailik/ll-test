using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Rewards.Models;
using LL.Saving;
using LL.User.Core;
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

        private readonly Dictionary<ItemId, int> _savedItemAmounts;
        private readonly HashSet<RewardBundleId> _savedClaimedRewardIds;
        private UserProgressData _savedProgress;
        private UserPromotionOrderData _savedPromotionOrder;

        [Inject]
        internal UserSaveController(
            UserData userData,
            IUserItems items,
            IUserProgress progress,
            IUserPromotionOrder promotionOrder,
            IUserRewardClaims rewardClaims,
            ISaveService saveService)
        {
            if (userData == null)
                throw new ArgumentNullException(nameof(userData));

            _identity = userData.Identity;
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _promotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            _rewardClaims = rewardClaims ?? throw new ArgumentNullException(nameof(rewardClaims));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));

            _savedItemAmounts = new Dictionary<ItemId, int>();

            foreach (var item in userData.Items.Amounts)
                _savedItemAmounts.Add(item.Id, item.Amount);

            _savedProgress = new UserProgressData(_progress.Rank, _progress.Experience);
            _savedPromotionOrder = new UserPromotionOrderData(
                _promotionOrder.RequirementId,
                _promotionOrder.DeadlineUnixMilliseconds,
                _promotionOrder.IsCompleted);
            _savedClaimedRewardIds = new HashSet<RewardBundleId>(_rewardClaims.ClaimedIds);
        }

        public void Initialize()
        {
            SubscribeToItemChanges();

            _subscriptions.Add(_progress.RankChanged.Subscribe(_ => OnProgressChanged()));
            _subscriptions.Add(_progress.ExperienceChanged.Subscribe(_ => OnProgressChanged()));
            _subscriptions.Add(_promotionOrder.Changed.Subscribe(_ => OnPromotionOrderChanged()));
            _subscriptions.Add(_rewardClaims.RewardClaimed.Subscribe(OnRewardClaimed));
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
        }

        private void SubscribeToItemChanges()
        {
            foreach (var itemId in _savedItemAmounts.Keys)
            {
                _subscriptions.Add(_items
                    .ObserveAmount(itemId)
                    .Subscribe(amount => OnItemAmountChanged(itemId, amount)));
            }
        }

        private void OnItemAmountChanged(ItemId itemId, int amount)
        {
            if (_savedItemAmounts[itemId] == amount)
                return;

            _savedItemAmounts[itemId] = amount;
            Save();
        }

        private void OnRewardClaimed(RewardBundleId rewardId)
        {
            if (_savedClaimedRewardIds.Add(rewardId))
                Save();
        }

        private void OnPromotionOrderChanged()
        {
            var promotionOrderData = new UserPromotionOrderData(
                _promotionOrder.RequirementId,
                _promotionOrder.DeadlineUnixMilliseconds,
                _promotionOrder.IsCompleted);

            if (_savedPromotionOrder.RequirementId.Equals(promotionOrderData.RequirementId) &&
                _savedPromotionOrder.DeadlineUnixMilliseconds == promotionOrderData.DeadlineUnixMilliseconds &&
                _savedPromotionOrder.IsCompleted == promotionOrderData.IsCompleted)
            {
                return;
            }

            _savedPromotionOrder = promotionOrderData;
            Save();
        }

        private void OnProgressChanged()
        {
            var progressData = new UserProgressData(
                _progress.Rank,
                _progress.Experience);

            if (_savedProgress.Rank == progressData.Rank &&
                _savedProgress.Experience == progressData.Experience)
            {
                return;
            }

            _savedProgress = progressData;
            Save();
        }

        private void Save()
        {
            var saveData = UserSaveDataMapper.ToSaveData(CreateUserData());
            _saveService.TrySave(UserDataLoader.SaveKey, saveData);
        }

        private UserData CreateUserData()
        {
            return new UserData(
                _identity,
                new UserItemsData(GetItemAmounts()),
                _savedProgress,
                _savedPromotionOrder,
                new UserRewardClaimsData(_savedClaimedRewardIds));
        }

        private IEnumerable<ItemAmount> GetItemAmounts()
        {
            foreach (var item in _savedItemAmounts)
                yield return new ItemAmount(item.Key, item.Value);
        }
    }
}