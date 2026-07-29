using System;
using System.Collections.Generic;
using LL.Game.Items;
using LL.Infrastructure.Saving;
using LL.User.Snapshots;
using LL.User.State.Items;
using LL.User.State.Progress;
using LL.User.State.Promotions;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.User.Persistence
{
    internal sealed class UserSaveController : IInitializable, IDisposable
    {
        private readonly UserIdentitySnapshot _identity;
        private readonly IUserItems _items;
        private readonly IUserProgress _progress;
        private readonly IUserPromotionOrder _promotionOrder;
        private readonly ISaveService _saveService;
        private readonly List<IDisposable> _subscriptions = new();

        private readonly Dictionary<ItemId, int> _itemAmounts;
        private UserProgressSnapshot _progressSnapshot;
        private UserPromotionOrderSnapshot _promotionOrderSnapshot;

        [Inject]
        internal UserSaveController(
            UserSnapshot initialSnapshot,
            IUserItems items,
            IUserProgress progress,
            IUserPromotionOrder promotionOrder,
            ISaveService saveService)
        {
            if (initialSnapshot == null)
                throw new ArgumentNullException(nameof(initialSnapshot));

            _identity = initialSnapshot.Identity;
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _promotionOrder = promotionOrder ?? throw new ArgumentNullException(nameof(promotionOrder));
            _saveService = saveService ?? throw new ArgumentNullException(nameof(saveService));

            _itemAmounts = new Dictionary<ItemId, int>();

            foreach (var item in initialSnapshot.Items.Amounts)
                _itemAmounts.Add(item.Id, item.Amount);

            _progressSnapshot = new UserProgressSnapshot(_progress.Rank, _progress.Experience);
            _promotionOrderSnapshot = new UserPromotionOrderSnapshot(
                _promotionOrder.RequirementId,
                _promotionOrder.DeadlineUnixMilliseconds,
                _promotionOrder.IsCompleted);
        }

        public void Initialize()
        {
            SubscribeToItemChanges();

            _subscriptions.Add(_progress.RankChanged.Subscribe(_ => OnProgressChanged()));
            _subscriptions.Add(_progress.ExperienceChanged.Subscribe(_ => OnProgressChanged()));
            _subscriptions.Add(_promotionOrder.Changed.Subscribe(_ => OnPromotionOrderChanged()));
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
        }

        private void SubscribeToItemChanges()
        {
            foreach (var itemId in _itemAmounts.Keys)
            {
                _subscriptions.Add(_items
                    .ObserveAmount(itemId)
                    .Subscribe(amount => OnItemAmountChanged(itemId, amount)));
            }
        }

        private void OnItemAmountChanged(ItemId itemId, int amount)
        {
            if (_itemAmounts[itemId] == amount)
                return;

            _itemAmounts[itemId] = amount;
            Save();
        }

        private void OnPromotionOrderChanged()
        {
            var snapshot = new UserPromotionOrderSnapshot(
                _promotionOrder.RequirementId,
                _promotionOrder.DeadlineUnixMilliseconds,
                _promotionOrder.IsCompleted);

            if (_promotionOrderSnapshot.RequirementId.Equals(snapshot.RequirementId) &&
                _promotionOrderSnapshot.DeadlineUnixMilliseconds == snapshot.DeadlineUnixMilliseconds &&
                _promotionOrderSnapshot.IsCompleted == snapshot.IsCompleted)
            {
                return;
            }

            _promotionOrderSnapshot = snapshot;
            Save();
        }

        private void OnProgressChanged()
        {
            var snapshot = new UserProgressSnapshot(
                _progress.Rank,
                _progress.Experience);

            if (_progressSnapshot.Rank == snapshot.Rank &&
                _progressSnapshot.Experience == snapshot.Experience)
            {
                return;
            }

            _progressSnapshot = snapshot;
            Save();
        }

        private void Save()
        {
            var saveData = UserSaveDataMapper.ToSaveData(CreateSnapshot());
            _saveService.TrySave(UserSnapshotLoader.SaveKey, saveData);
        }

        private UserSnapshot CreateSnapshot()
        {
            return new UserSnapshot(
                _identity,
                new UserItemsSnapshot(GetItemAmounts()),
                _progressSnapshot,
                _promotionOrderSnapshot);
        }

        private IEnumerable<ItemAmount> GetItemAmounts()
        {
            foreach (var item in _itemAmounts)
                yield return new ItemAmount(item.Key, item.Value);
        }
    }
}